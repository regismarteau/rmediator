<#
.SYNOPSIS
  Garde-fou Claude Code : refuse `git commit` tant que /review-modified-files n'a pas été passé, formatage et tests verts, sur l'état courant du code.
.NOTES
  Sans argument : hook PreToolUse sur Bash, déclaré dans .claude/settings.json du projet par sync.ps1 (JSON sur stdin ; code 2 = appel refusé, stderr est renvoyé à Claude).
  Sans effet pour un commit sans code.
  Le dépôt contrôlé est le `cwd` du JSON du hook (le worktree de la session).
  -Mark : appelé par le skill review-modified-files en fin de revue ; lance dotnet format sur les fichiers modifiés puis dotnet test sur la solution,
  et n'enregistre l'empreinte de l'état courant (stockée dans le dossier git, jamais commitée) que si tout passe.
#>
param([switch]$Mark)

$ErrorActionPreference = 'Continue'
$codePattern = '\.(cs|razor|cshtml|csproj|props|sql|ts|tsx|js|css|scss|html|htm|feature)$'

$raw = if ($Mark) { '' } else { [Console]::In.ReadToEnd() }
$payload = try { $raw | ConvertFrom-Json } catch { $null }
$start = if ($payload.cwd) { $payload.cwd } elseif ($env:CLAUDE_PROJECT_DIR) { $env:CLAUDE_PROJECT_DIR } else { (Get-Location).Path }
$root = git -C $start rev-parse --show-toplevel 2>$null
if (-not $root) { exit 0 }

function Get-ChangedFiles {
    @(git -C $root -c core.quotepath=off diff HEAD --name-only --diff-filter=d) + @(git -C $root -c core.quotepath=off ls-files --others --exclude-standard) | Where-Object { $_ }
}

function Get-Fingerprint {
    $parts = @(git -C $root diff HEAD)
    foreach ($file in @(git -C $root -c core.quotepath=off ls-files --others --exclude-standard)) {
        $parts += "$file $(git -C $root hash-object -- $file)"
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes(($parts -join "`n"))
    ([BitConverter]::ToString([Security.Cryptography.SHA256]::Create().ComputeHash($bytes))) -replace '-'
}

$markerPath = git -C $root rev-parse --git-path claude-review-fingerprint
if (-not [IO.Path]::IsPathRooted($markerPath)) { $markerPath = Join-Path $root $markerPath }

if ($Mark) {
    Remove-Item $markerPath -ErrorAction SilentlyContinue
    $solution = Get-ChildItem $root -File | Where-Object { $_.Extension -in '.sln', '.slnx' } | Select-Object -First 1
    if ($solution) {
        Push-Location $root
        try {
            $sources = @(Get-ChangedFiles | Where-Object { $_ -match '\.(cs|vb)$' })
            if ($sources) {
                dotnet format $solution.FullName --severity info --include @sources
                if ($LASTEXITCODE -ne 0) { [Console]::Error.WriteLine('dotnet format a échoué : corriger puis relancer.'); exit 1 }
            }
            $output = dotnet test $solution.FullName 2>&1
            if ($LASTEXITCODE -ne 0) {
                $output | Select-Object -Last 80 | ForEach-Object { [Console]::Error.WriteLine($_) }
                [Console]::Error.WriteLine('dotnet test a échoué : corriger puis relancer.')
                exit 1
            }
            $output | Select-Object -Last 3 | Write-Output
        }
        finally { Pop-Location }
    }
    else { Write-Output 'aucune solution .NET à la racine : formatage et tests du projet à lancer à la main.' }
    [IO.File]::WriteAllText($markerPath, (Get-Fingerprint))
    Write-Output 'revue enregistrée : le commit est autorisé tant que les fichiers ne changent pas.'
    exit 0
}

$command = $payload.tool_input.command
if (-not $command -or $command -notmatch '(?m)(^|[;&|]\s*)git(\s+-[-\w]+(\s+[^-\s]\S*)?)*\s+commit(\s|$)') { exit 0 }
if (-not (Get-ChangedFiles | Where-Object { $_ -match $codePattern })) { exit 0 }

$reviewed = if (Test-Path $markerPath) { [IO.File]::ReadAllText($markerPath).Trim() } else { '' }
if ($reviewed -eq (Get-Fingerprint)) { exit 0 }

[Console]::Error.WriteLine('Commit refusé : le skill /review-modified-files n''a pas été passé sur l''état actuel des fichiers (revue clean code, formatage, tests). Le lancer, puis recommiter. Toute modification après la revue demande de la relancer.')
exit 2

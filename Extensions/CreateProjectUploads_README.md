# CreateProjectUploads

## Usage

Place these files in:

```text
Extensions\
```

Run from the solution root:

```text
CreateProjectUploads.bat
```

or:

```powershell
.\CreateProjectUploads.ps1
```

## Output

ZIP files are created here:

```text
Extensions\Upload
```

Example:

```text
Extensions\Upload\CodexExpensa.ExtensionDevHost.upload.zip
```

## Skips

```text
bin
obj
.vs
.git
.vscode
packages
node_modules
TestResults
Upload
```

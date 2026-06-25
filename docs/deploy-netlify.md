# Deploy no Netlify

## Opcao 1: publicar pelo repositorio

O projeto ja esta preparado com `netlify.toml`.

- Build command: `dotnet publish pRHosaApp1.csproj -c Release`
- Publish directory: `bin/Release/net9.0/publish/wwwroot`

Tambem foi adicionada a regra SPA em `wwwroot/_redirects`, para evitar erro `404` ao abrir rotas como `/login`, `/home`, `/Necessidades` e similares.

## Opcao 2: publicar manualmente

Execute localmente:

```powershell
dotnet publish .\pRHosaApp1.csproj -c Release
```

Depois envie para o Netlify o conteudo da pasta:

```text
bin/Release/net9.0/publish/wwwroot
```

## Observacoes

- A API continua apontando para `wwwroot/appsettings.json`.
- Se o deploy pelo repositorio falhar por SDK do .NET no ambiente do Netlify, use a opcao manual com a pasta `publish/wwwroot`.

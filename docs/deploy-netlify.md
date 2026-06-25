# Deploy no Netlify

## Opcao 1: publicar pelo GitHub Actions

O caminho mais confiavel para este projeto e usar GitHub Actions para compilar o Blazor WebAssembly e enviar o artefato pronto ao Netlify.

Ja deixamos o workflow em:

```text
.github/workflows/deploy-netlify.yml
```

Voce precisa cadastrar dois secrets no GitHub:

- `NETLIFY_AUTH_TOKEN`
- `NETLIFY_SITE_ID`

Com isso, todo push em `main` passa a publicar automaticamente no Netlify.

## Opcao 2: publicar pelo repositorio no Netlify

O projeto ja esta preparado com `netlify.toml`.

- Build command: `bash ./tools/netlify-build.sh`
- Publish directory: `bin/Release/net9.0/publish/wwwroot`

Tambem foi adicionada a regra SPA em `wwwroot/_redirects`, para evitar erro `404` ao abrir rotas como `/login`, `/home`, `/Necessidades` e similares.

Como o ambiente padrao do Netlify iniciou o build com .NET 8, deixamos um script dedicado que instala o SDK .NET 9 antes do `publish`.

Observacao importante:

O Netlify hoje nao lista .NET entre as linguagens disponiveis no ambiente padrao de build. Por isso, para projetos Blazor WebAssembly em `net9.0`, o fluxo por GitHub Actions tende a ser o mais estavel.

## Opcao 3: publicar manualmente

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
- Se o deploy pelo repositorio falhar no Netlify, prefira o workflow do GitHub Actions ou use a opcao manual com a pasta `publish/wwwroot`.

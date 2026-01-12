module.exports = {
  apps: [
    {
      name: 'yasam-psikolog-api',
      script: 'dotnet',
      args: 'YasamPsikologProject.WebApi.dll',
      cwd: '/var/www/yasam-psikolog/api',
      interpreter: 'none',
      env: {
        ASPNETCORE_ENVIRONMENT: 'Production',
        ASPNETCORE_URLS: 'http://localhost:5003',
        DOTNET_PRINT_TELEMETRY_MESSAGE: 'false'
      },
      error_file: '/var/log/pm2/yasam-api-error.log',
      out_file: '/var/log/pm2/yasam-api-out.log',
      log_date_format: 'YYYY-MM-DD HH:mm:ss Z',
      merge_logs: true,
      autorestart: true,
      watch: false,
      max_memory_restart: '500M',
      instances: 1,
      exec_mode: 'fork'
    },
    {
      name: 'yasam-psikolog-frontend',
      script: 'dotnet',
      args: 'YasamPsikologProject.WebUi.dll',
      cwd: '/var/www/yasam-psikolog/frontend',
      interpreter: 'none',
      env: {
        ASPNETCORE_ENVIRONMENT: 'Production',
        ASPNETCORE_URLS: 'http://localhost:5004',
        DOTNET_PRINT_TELEMETRY_MESSAGE: 'false'
      },
      error_file: '/var/log/pm2/yasam-frontend-error.log',
      out_file: '/var/log/pm2/yasam-frontend-out.log',
      log_date_format: 'YYYY-MM-DD HH:mm:ss Z',
      merge_logs: true,
      autorestart: true,
      watch: false,
      max_memory_restart: '300M',
      instances: 1,
      exec_mode: 'fork'
    }
    // Mevcut diğer projeleriniz buraya eklenebilir
    // {
    //   name: 'existing-project',
    //   script: 'dotnet',
    //   args: 'ExistingProject.dll',
    //   ...
    // }
  ]
};

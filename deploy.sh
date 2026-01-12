#!/bin/bash

# Yaşam Psikoloji Projesi Deployment Script
# Ubuntu Server - PM2 ile deployment

set -e

echo "====================================="
echo "Yaşam Psikoloji Deployment Başlıyor"
echo "====================================="

# Renkler
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Değişkenler
PROJECT_NAME="yasam-psikolog"
SERVER_USER="root"
SERVER_IP="93.177.102.34"
SERVER_PATH="/var/www/${PROJECT_NAME}"
API_PATH="${SERVER_PATH}/api"
FRONTEND_PATH="${SERVER_PATH}/frontend"
API_PORT="5003"  # Mevcut 5001 kullanımda, 5003 kullanalım
FRONTEND_PORT="5004"

# 1. Projeyi Build Et
echo -e "${YELLOW}[1/7] Projeleri build ediliyor...${NC}"
dotnet publish YasamPsikologProject.API/YasamPsikologProject.WebApi/YasamPsikologProject.WebApi.csproj \
    -c Release \
    -o ./publish/api \
    --self-contained false

dotnet publish YasamPsikologProject.Frontend/YasamPsikologProject.WebUi/YasamPsikologProject.WebUi.csproj \
    -c Release \
    -o ./publish/frontend \
    --self-contained false

echo -e "${GREEN}✓ Build tamamlandı${NC}"

# 2. Server'da klasörleri oluştur
echo -e "${YELLOW}[2/7] Server'da dizinler oluşturuluyor...${NC}"
ssh ${SERVER_USER}@${SERVER_IP} "mkdir -p ${API_PATH} ${FRONTEND_PATH}"
echo -e "${GREEN}✓ Dizinler oluşturuldu${NC}"

# 3. Dosyaları sunucuya kopyala
echo -e "${YELLOW}[3/7] Dosyalar sunucuya aktarılıyor...${NC}"
rsync -avz --delete ./publish/api/ ${SERVER_USER}@${SERVER_IP}:${API_PATH}/
rsync -avz --delete ./publish/frontend/ ${SERVER_USER}@${SERVER_IP}:${FRONTEND_PATH}/
echo -e "${GREEN}✓ Dosyalar aktarıldı${NC}"

# 4. Veritabanı dosyasını kopyala
echo -e "${YELLOW}[4/7] Veritabanı dosyası aktarılıyor...${NC}"
rsync -avz ./YasamPsikologDb/YasamPsikologDb.bak ${SERVER_USER}@${SERVER_IP}:${SERVER_PATH}/
echo -e "${GREEN}✓ Veritabanı dosyası aktarıldı${NC}"

# 5. Docker Compose dosyasını kopyala ve SQL Server'ı başlat
echo -e "${YELLOW}[5/7] SQL Server container başlatılıyor...${NC}"
rsync -avz ./docker-compose.yml ${SERVER_USER}@${SERVER_IP}:${SERVER_PATH}/
ssh ${SERVER_USER}@${SERVER_IP} "cd ${SERVER_PATH} && docker-compose up -d"
echo -e "${GREEN}✓ SQL Server başlatıldı (Port: 1434)${NC}"

# 6. PM2 Ecosystem dosyasını kopyala
echo -e "${YELLOW}[6/7] PM2 konfigürasyonu güncelleniyor...${NC}"
rsync -avz ./ecosystem.config.js ${SERVER_USER}@${SERVER_IP}:${SERVER_PATH}/

# 7. PM2 ile uygulamaları başlat/yeniden başlat
echo -e "${YELLOW}[7/7] Uygulamalar PM2 ile başlatılıyor...${NC}"
ssh ${SERVER_USER}@${SERVER_IP} << 'ENDSSH'
cd /var/www/yasam-psikolog

# PM2 ecosystem dosyasını güncelle
cat > ecosystem.config.js << 'EOF'
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
  ]
};
EOF

# PM2'yi güncelle
pm2 delete yasam-psikolog-api 2>/dev/null || true
pm2 delete yasam-psikolog-frontend 2>/dev/null || true
pm2 start ecosystem.config.js
pm2 save

echo "PM2 uygulamaları başlatıldı"
pm2 list

ENDSSH

echo -e "${GREEN}✓ PM2 uygulamaları başlatıldı${NC}"

echo ""
echo -e "${GREEN}====================================="
echo "Deployment Tamamlandı!"
echo "=====================================${NC}"
echo ""
echo "Uygulamalar:"
echo "  - API: http://localhost:5003"
echo "  - Frontend: http://localhost:5004"
echo "  - SQL Server: localhost:1434"
echo ""
echo "PM2 Durumunu Kontrol Et:"
echo "  ssh ${SERVER_USER}@${SERVER_IP} 'pm2 list'"
echo ""
echo "Logları Görüntüle:"
echo "  ssh ${SERVER_USER}@${SERVER_IP} 'pm2 logs yasam-psikolog-api'"
echo "  ssh ${SERVER_USER}@${SERVER_IP} 'pm2 logs yasam-psikolog-frontend'"
echo ""
echo -e "${YELLOW}⚠ Nginx konfigürasyonunu güncellemeyi unutmayın!${NC}"
echo "Örnek: cat nginx-config-example.conf"

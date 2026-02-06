# Yaşam Psikoloji Projesi Deployment Script
# Ubuntu Server - PM2 ile deployment

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Yaşam Psikoloji Deployment Başlıyor" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

# Değişkenler
$PROJECT_NAME = "yasam-psikolog"
$SERVER_USER = "root"
$SERVER_IP = "93.177.102.34"
$SERVER_PATH = "/var/www/$PROJECT_NAME"
$API_PATH = "$SERVER_PATH/api"
$FRONTEND_PATH = "$SERVER_PATH/frontend"

# 1. Server'da klasörleri oluştur
Write-Host "[1/5] Server'da dizinler oluşturuluyor..." -ForegroundColor Yellow
ssh ${SERVER_USER}@${SERVER_IP} "mkdir -p ${API_PATH} ${FRONTEND_PATH}"
Write-Host "✓ Dizinler oluşturuldu" -ForegroundColor Green

# 2. Dosyaları sunucuya kopyala
Write-Host "[2/5] API dosyaları sunucuya aktarılıyor..." -ForegroundColor Yellow
scp -r d:\Projeler\yasam_psikoloji\publish\api\* ${SERVER_USER}@${SERVER_IP}:${API_PATH}/
Write-Host "✓ API dosyaları aktarıldı" -ForegroundColor Green

Write-Host "[3/5] Frontend dosyaları sunucuya aktarılıyor..." -ForegroundColor Yellow
scp -r d:\Projeler\yasam_psikoloji\publish\frontend\* ${SERVER_USER}@${SERVER_IP}:${FRONTEND_PATH}/
Write-Host "✓ Frontend dosyaları aktarıldı" -ForegroundColor Green

# 3. Backup dosyasını sunucuya kopyala
Write-Host "[4/5] Veritabanı backup'ı sunucuya aktarılıyor..." -ForegroundColor Yellow
docker cp $(docker ps -q --filter "ancestor=mcr.microsoft.com/mssql/server"):/var/opt/mssql/data/YasamPsikologDb_Clean_20260122_173026.bak ./YasamPsikologDb_Clean.bak
scp ./YasamPsikologDb_Clean.bak ${SERVER_USER}@${SERVER_IP}:/tmp/
Remove-Item ./YasamPsikologDb_Clean.bak
Write-Host "✓ Backup dosyası aktarıldı" -ForegroundColor Green

# 4. PM2 ile servisleri başlat
Write-Host "[5/5] Sunucuda servisler başlatılıyor..." -ForegroundColor Yellow
$remoteCommands = @'
pm2 delete yasam-api 2>/dev/null; true
pm2 delete yasam-frontend 2>/dev/null; true

cd /var/www/yasam-psikolog/api
pm2 start "dotnet YasamPsikologProject.WebApi.dll --urls http://0.0.0.0:5003" --name yasam-api

cd /var/www/yasam-psikolog/frontend
pm2 start "dotnet YasamPsikologProject.WebUi.dll --urls http://0.0.0.0:5004" --name yasam-frontend

pm2 save

docker exec -i $(docker ps -q --filter "publish=1434") /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd123" -Q "RESTORE DATABASE YasamPsikologDb FROM DISK = '/tmp/YasamPsikologDb_Clean.bak' WITH REPLACE, RECOVERY"

echo "Deployment tamamlandi!"
'@

ssh ${SERVER_USER}@${SERVER_IP} $remoteCommands

Write-Host "" 
Write-Host "=====================================" -ForegroundColor Green
Write-Host "✓ Deployment Başarıyla Tamamlandı!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Servisler:" -ForegroundColor Cyan
Write-Host "  API:      http://93.177.102.34:5003" -ForegroundColor White
Write-Host "  Frontend: http://93.177.102.34:5004" -ForegroundColor White
Write-Host ""
Write-Host "Giriş Bilgileri:" -ForegroundColor Cyan
Write-Host "  Email:    superadmin@gmail.com" -ForegroundColor White
Write-Host "  Password: Admin123" -ForegroundColor White

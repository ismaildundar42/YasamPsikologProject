# Yaşam Psikoloji Projesi - Deployment Rehberi

## Sunucu Bilgileri
- **IP:** 93.177.102.34
- **Kullanıcı:** root
- **OS:** Ubuntu Server

## Mevcut Durum
- **Çalışan Projeler:** onus.com.tr (Port 3000: Frontend, Port 5001: API)
- **PM2:** Aktif (onus-frontend çalışıyor)
- **Nginx:** Aktif (Port 80, 443)
- **PostgreSQL:** Port 5432 (localhost)
- **Docker:** Kurulu değil

## Yeni Proje Portları
- **API:** Port 5003
- **Frontend:** Port 5004
- **SQL Server:** Port 1434 (Docker ile)

## Deployment Adımları

### 1. Docker Kurulumu (İlk Defa)
```bash
ssh root@93.177.102.34

# Docker kurulumu
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Docker Compose kurulumu
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

### 2. Deployment Script'i Çalıştır
```bash
# Windows PowerShell'den:
bash deploy.sh
```

Deploy script otomatik olarak:
- ✅ Projeleri build eder
- ✅ Dosyaları sunucuya kopyalar
- ✅ SQL Server container'ını başlatır
- ✅ PM2 ile uygulamaları başlatır

### 3. SQL Server Veritabanını Restore Et
```bash
ssh root@93.177.102.34

# Container'a bağlan
docker exec -it yasam_psikolog_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd123'

# Veritabanını restore et
RESTORE DATABASE YasamPsikologDb 
FROM DISK = '/var/opt/mssql/backup/YasamPsikologDb.bak'
WITH MOVE 'YasamPsikologDb' TO '/var/opt/mssql/data/YasamPsikologDb.mdf',
     MOVE 'YasamPsikologDb_log' TO '/var/opt/mssql/data/YasamPsikologDb_log.ldf'
GO
```

### 4. Nginx Konfigürasyonu

#### Seçenek A: Subdomain ile (Tavsiye Edilen)
```bash
ssh root@93.177.102.34

# Yeni site config dosyası oluştur
sudo nano /etc/nginx/sites-available/yasam-psikolog

# nginx-config-example.conf içeriğini yapıştır (SEÇENEK 1)
# Dosyayı kaydet ve çık

# Site'ı aktifleştir
sudo ln -s /etc/nginx/sites-available/yasam-psikolog /etc/nginx/sites-enabled/

# Nginx test et
sudo nginx -t

# Nginx'i yeniden başlat
sudo systemctl reload nginx
```

#### Seçenek B: Mevcut onus.com.tr içine path-based routing
```bash
ssh root@93.177.102.34

# Mevcut config'i düzenle
sudo nano /etc/nginx/sites-available/onus

# nginx-config-example.conf içindeki SEÇENEK 2'yi ekle
# location /yasam/ ve location /yasam-api/ bloklarını ekle

# Nginx test et
sudo nginx -t

# Nginx'i yeniden başlat
sudo systemctl reload nginx
```

### 5. SSL Sertifikası (Subdomain için - Opsiyonel)
```bash
ssh root@93.177.102.34

# Certbot ile SSL
sudo certbot --nginx -d yasampsikoloj.com -d www.yasampsikoloj.com -d api.yasampsikoloj.com
```

## Kontrol Komutları

### PM2 Durumu
```bash
ssh root@93.177.102.34 'pm2 list'
ssh root@93.177.102.34 'pm2 logs yasam-psikolog-api'
ssh root@93.177.102.34 'pm2 logs yasam-psikolog-frontend'
```

### Docker Durumu
```bash
ssh root@93.177.102.34 'docker ps'
ssh root@93.177.102.34 'docker logs yasam_psikolog_sqlserver'
```

### Nginx Durumu
```bash
ssh root@93.177.102.34 'sudo nginx -t'
ssh root@93.177.102.34 'sudo systemctl status nginx'
```

### Port Kontrolü
```bash
ssh root@93.177.102.34 'ss -tulpn | grep LISTEN'
```

## Güncelleme için
```bash
# deploy.sh script'ini tekrar çalıştırın
bash deploy.sh
```

## Sorun Giderme

### PM2 Logları
```bash
ssh root@93.177.102.34 'pm2 logs yasam-psikolog-api --lines 100'
ssh root@93.177.102.34 'pm2 logs yasam-psikolog-frontend --lines 100'
```

### SQL Server Bağlantı Testi
```bash
ssh root@93.177.102.34 'docker exec yasam_psikolog_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd123" -Q "SELECT @@VERSION"'
```

### Uygulama Yeniden Başlatma
```bash
ssh root@93.177.102.34 'pm2 restart yasam-psikolog-api'
ssh root@93.177.102.34 'pm2 restart yasam-psikolog-frontend'
```

## Önemli Notlar

1. **Portlar:** Mevcut onus.com.tr projesi 3000 ve 5001 portlarını kullanıyor. Yeni proje 5003 ve 5004 kullanıyor.

2. **SQL Server Şifresi:** Production'da `YourStrong@Passw0rd123` şifresini değiştirin ve appsettings.Production.json'ı güncelleyin.

3. **CORS:** API'de CORS ayarlarını domain'inize göre güncelleyin.

4. **PM2:** Sunucu yeniden başladığında otomatik başlat:
   ```bash
   ssh root@93.177.102.34 'pm2 startup'
   ssh root@93.177.102.34 'pm2 save'
   ```

5. **Docker Compose:** Sunucu yeniden başladığında SQL Server otomatik başlayacak (restart: unless-stopped).

## Erişim Adresleri

### Subdomain kullanıyorsanız:
- Frontend: http://yasampsikoloj.com
- API: http://api.yasampsikoloj.com

### Path-based kullanıyorsanız:
- Frontend: http://onus.com.tr/yasam/
- API: http://onus.com.tr/yasam-api/

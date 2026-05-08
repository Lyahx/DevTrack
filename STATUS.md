# DevTrack — Durum Snapshot

Son güncelleme: 2026-05-08

Bu dosya projeye geri döndüğünde 30 saniyede yerini bulman için. Detaylı doküman `README.md`.

## Şu an nerede

- **Prompt 1, 2, 3 tamamı bitti.** Backend (.NET 10 + **PostgreSQL 16**) Docker'da, frontend (Next.js 16) `npm run dev` ile.
- **DB taşıma 2026-05-08:** MSSQL → PostgreSQL (Npgsql). Mevcut data **silindi**, sıfırdan migration oluşturuluyor.
- Tarayıcıdan **http://localhost:3000** → register/login → dashboard çalışıyor.
- Son test: dev quick-login + mock seed + dashboard + Resume Mode + commit listesi → hepsi yeşil.

## Sıradaki — UI rafine

Kullanıcının kendi dediği: _"genel tasarım mantığı güzel ama biraz daha göze güzel hale getirebiliriz"_. Yapılacaklar henüz seçilmedi; geri dönüldüğünde hangi sayfa/bileşen göze batıyorsa oradan başlanacak. Aday alanlar (henüz konuşulmadı, varsayım):

- Dashboard kart yoğunluğu — özellikle stale projects banner + active grid + recent worklogs aynı ekranda biraz hızlı.
- Sidebar genişliği ve typography hiyerarşisi.
- Detail panel boşken hâlâ biraz çıplak; "today's open next steps" tek başına yetiyor mu?
- Resume Mode'da büyük tipografi var ama emoji'li başlıklar (📍 📝 ✅ 💡 🔗 ✨) tasarımcı gözüyle karışık olabilir.
- Header ortalanmış search trigger ile sağ ikonların hizası.

## Son turlardaki düzeltmeler (sırasıyla)

1. **CORS** + Scalar/OpenAPI **AllowAnonymous** — frontend artık API'a doğrudan ulaşıyor (`Program.cs`).
2. **`IReminderGenerator` DI kaydı** atlanmıştı (Prompt 2 kalıntısı), eklendi (`Service/ServiceCollectionExtensions.cs`).
3. **Dev araçları:**
   - `POST /api/v1/dev/quick-login` — `dev` kullanıcısını idempotent oluşturup token döner. Sadece Development.
   - `POST /api/v1/dev/seed` — kullanıcının verisini siler, kapsamlı mock dataset basar (5 proje + 6 bileşen + 3 eğitim + 8 modül + 9 worklog + 10 next-step + 6 fikir + 9 kaynak + reminder).
   - `POST /api/v1/dev/wipe` — sadece temizler.
   - Frontend: `/login`'da **"Dev: hızlı giriş"** butonu, `/settings`'te **"Mock data yükle / Tümünü sil"**.
4. **Dashboard 500'ü düzeltildi** — `DashboardService.GetAsync` paralel `Task.WhenAll` kullanıyordu, aynı scoped `DbContext` thread'lerden çağırıldığı için "second operation started on this context" patlatıyordu. Sequential `await`'e çevrildi.
5. **GitHub commit takibi:**
   - `GET /api/v1/projects/{id}/commits?limit=N` — repo URL'i parse eder (sadece `github.com`), `IHttpClientFactory` + `IMemoryCache` (5 dk) ile çeker.
   - `GitHubCommitService` (`src/DevTrack.Service/Implementations/`).
   - Frontend: `CommitList` component, project detail Overview tab'ının başında.
   - Mock data'da DevTrack → `dotnet/efcore`, AOS → `vercel/next.js` (test için).

## Çalıştırma (geri döndüğünde)

```powershell
# 1. Docker Desktop'ı aç (sistem tepsisinden)
# 2. Backend
cd C:\Users\MSI\Desktop\DevTrack
docker compose up -d            # postgres + api ayağa kalkar (~10 sn)

# 3. Frontend (ayrı terminal)
cd web
npm run dev                     # http://localhost:3000

# 4. Tarayıcıda /login → "Dev: hızlı giriş" → /settings → "Mock data yükle"
```

**PostgreSQL ilk kurulum (ya da yeniden migration gerektiğinde):**

```powershell
# Eski MSSQL container/volume'ünü temizle (varsa)
docker compose down -v

# Postgres'i ayağa kaldır
docker compose up -d postgres

# Migration oluştur (host üzerinde dotnet kuruluysa)
dotnet ef migrations add InitialCreate `
  -p src/DevTrack.Infrastructure -s src/DevTrack.Api

# API ayağa kalkınca DEVTRACK_AUTO_MIGRATE=true sayesinde migration uygulanır
docker compose up -d api
```

`.env` repo kökünde mevcut (kapatma sırasında silinmez, yeniden generate gerekmez).

## AI ile içe aktar (LM Studio / Ollama)

- LearningTrack form'una `aiChatUrl` eklendi (Claude/ChatGPT share link, opsiyonel referans).
- `POST /api/v1/learning-tracks/{id}/ai-import` — transcript yapıştırılır → yerel AI provider (LM Studio / Ollama) `worklogs / decisions / nextSteps / ideas / resources` extract eder.
- `POST .../ai-import/apply` — kullanıcının seçtiği item'ları tek transaction'da kaydeder.
- Eğitim detay sayfası "Genel" tab üstünde violet hero kart + "İçe aktar" butonu.
- Provider config docker-compose `Ai__BaseUrl` / `Ai__Model` / `Ai__ApiKey` env'leri (.env'de `AI_BASE_URL` vb.). Default LM Studio `http://host.docker.internal:1234/v1`, model `llama-3.1-8b-instruct`.
- LM Studio'yu host'ta çalıştır → "Server" sekmesinden modeli yükle ve "Start Server" → DevTrack hazır.

## Bilinen pürüzler / TODO

- Soft-restore (geri yükleme) hâlâ yok — `/trash` sayfası kullanıcıya bunu söylüyor.
- Reminder generator daily 03:00 UTC çalışıyor, ama dev'de manuel tetikleyince bir kerelik tek reminder geliyor (mock seed sırasında 1 stale tracking var).
- `CORS:AllowedOrigins` tek origin (`http://localhost:3000`); başka frontend host'undan denemek için env var değiştir.
- DB artık PostgreSQL — eski MSSQL connection string'i / volume'ü kalmadı. Eski mock data yok, `/settings` → "Mock data yükle" ile yeniden bas.
- shadcn/ui artık **Base UI** kullanıyor (Radix değil) — `asChild` yok, `render` prop var. Yeni UI değişiklikleri yaparken bu fark akılda tutulsun.
- AutoMapper **16** (commercial license alanına yakın) — gerekirse Mapperly'ye geçilebilir, ama şu an çalışıyor.

## Repo yapısı kısa özet

```
DevTrack/
├── DevTrack.sln
├── docker-compose.yml
├── .env                         (gitignored, gerçek secrets içerir)
├── .env.example                 (commit-safe template)
├── README.md                    (uzun döküman)
├── STATUS.md                    (bu dosya)
├── src/                         (backend; 5 proje)
│   ├── DevTrack.Api
│   ├── DevTrack.Service
│   ├── DevTrack.Repository
│   ├── DevTrack.Domain
│   └── DevTrack.Infrastructure
└── web/                         (frontend; ~130 TS dosyası)
    ├── app/(public|app)/...
    ├── components/{layout,projects,components,learning,...}
    ├── lib/api/, lib/date.ts, lib/api.ts
    ├── store/{auth,quickCapture,commandPalette}.ts
    └── types/
```

## Klavye kısayolları

| | |
|---|---|
| `Ctrl/⌘ + K` | Komut paleti (navigate) |
| `Ctrl/⌘ + J` | Hızlı yakala |
| `Ctrl/⌘ + Enter` | Hızlı yakala — gönder |

## Test hesabı

Quick-login butonu kullandığında: `dev` / `devpass123` / `dev@devtrack.local`. Şifre burada açık çünkü sadece Development environment'ta çalışıyor.

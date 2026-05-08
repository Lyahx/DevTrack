using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Infrastructure.Data;
using DevTrack.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Service.Implementations;

public class DevSeedService : IDevSeedService
{
    private readonly DevTrackDbContext _db;
    private readonly IReminderGenerator _reminderGenerator;

    public DevSeedService(DevTrackDbContext db, IReminderGenerator reminderGenerator)
    {
        _db = db;
        _reminderGenerator = reminderGenerator;
    }

    public async Task<DevSeedResult> SeedAsync(int userId, CancellationToken ct = default)
    {
        await WipeAsync(userId, ct);

        var now = DateTime.UtcNow;
        var result = new DevSeedResult();

        // ─── Tags ─────────────────────────────────────────────────────────────
        var tagBackend = new Tag { UserId = userId, Name = "backend", Color = "#3B82F6" };
        var tagFrontend = new Tag { UserId = userId, Name = "frontend", Color = "#10B981" };
        var tagMvp = new Tag { UserId = userId, Name = "mvp", Color = "#F59E0B" };
        var tagClaude = new Tag { UserId = userId, Name = "claude", Color = "#F97316" };
        var tagRefactor = new Tag { UserId = userId, Name = "refactor", Color = "#A78BFA" };
        await _db.Tags.AddRangeAsync(new[] { tagBackend, tagFrontend, tagMvp, tagClaude, tagRefactor }, ct);
        await _db.SaveChangesAsync(ct);
        result.Tags = 5;

        // ─── Project 1: AOS (Active, recent activity) ─────────────────────────
        var aos = new Project
        {
            UserId = userId,
            Name = "AOS",
            Description = "Akademik Otomasyon Sistemi — öğrenci işleri, ders kayıt, transkript modülleri.",
            Goal = "Bitirme projesi olarak çalışan bir okul yönetim sistemi çıkarmak.",
            Status = ProjectStatus.Active,
            RepoUrl = "https://github.com/vercel/next.js",
        };
        await _db.Projects.AddAsync(aos, ct);
        await _db.SaveChangesAsync(ct);
        result.Projects++;

        var aosApi = new Component
        {
            ProjectId = aos.Id,
            Name = "AOS.Api",
            Type = ComponentType.Api,
            TechStack = ".NET 10, EF Core, PostgreSQL",
            LocalUrl = "http://localhost:5050",
            RepoPath = "src/AOS.Api",
            CurrentStatusNote = "Refresh token endpoint'ini eklemeye başladım. Validator yarıda kaldı — özellikle `RefreshTokenRequest` için MaxLength sınırını netleştirmem lazım. Yarın access token rotation'ı tamamla.",
        };
        var aosWeb = new Component
        {
            ProjectId = aos.Id,
            Name = "AOS.Web",
            Type = ComponentType.WebApp,
            TechStack = "Next.js 16, TanStack Query",
            LocalUrl = "http://localhost:3001",
            RepoPath = "src/AOS.Web",
            CurrentStatusNote = "Ders kayıt akışında prerequisite kontrolü çalışıyor. Sıra: çakışan saatler için UI uyarısı.",
        };
        await _db.Components.AddRangeAsync(new[] { aosApi, aosWeb }, ct);
        await _db.SaveChangesAsync(ct);
        result.Components += 2;

        await _db.ProjectTags.AddRangeAsync(new[]
        {
            new ProjectTag { ProjectId = aos.Id, TagId = tagBackend.Id },
            new ProjectTag { ProjectId = aos.Id, TagId = tagMvp.Id },
        }, ct);
        await _db.ComponentTags.AddRangeAsync(new[]
        {
            new ComponentTag { ComponentId = aosApi.Id, TagId = tagBackend.Id },
            new ComponentTag { ComponentId = aosWeb.Id, TagId = tagFrontend.Id },
        }, ct);

        // ─── Project 2: FoodTracker (Active, slightly stale at 8 days) ────────
        var food = new Project
        {
            UserId = userId,
            Name = "FoodTracker",
            Description = "Yediklerini barkod taratarak kaydeden mobil uygulama.",
            Goal = "Günlük kalori takibini parmaklarımı kıracak şekilde kolaylaştırmak.",
            Status = ProjectStatus.Active,
        };
        await _db.Projects.AddAsync(food, ct);
        await _db.SaveChangesAsync(ct);
        result.Projects++;

        var foodMobile = new Component
        {
            ProjectId = food.Id,
            Name = "FoodTracker.Mobile",
            Type = ComponentType.Mobile,
            TechStack = "React Native, Expo",
            CurrentStatusNote = "Barkod tarama lib'i kuruldu, gerçek bir ürün tarayıp Open Food Facts'tan çekme akışı çalışıyor. Offline cache henüz yok.",
        };
        await _db.Components.AddAsync(foodMobile, ct);
        await _db.SaveChangesAsync(ct);
        result.Components++;

        await _db.ProjectTags.AddAsync(new ProjectTag { ProjectId = food.Id, TagId = tagMvp.Id }, ct);

        // ─── Project 3: DevTrack (Active, fresh — meta) ───────────────────────
        var devtrack = new Project
        {
            UserId = userId,
            Name = "DevTrack",
            Description = "Şu an üzerinde durduğun proje. Kendini takip etsin diye.",
            Goal = "Başladığım projeleri ve eğitimleri kaybetmemek.",
            Status = ProjectStatus.Active,
            // Gerçek bir public repo — commit listesi out-of-the-box dolu görünsün diye.
            RepoUrl = "https://github.com/dotnet/efcore",
        };
        await _db.Projects.AddAsync(devtrack, ct);
        await _db.SaveChangesAsync(ct);
        result.Projects++;

        var dtApi = new Component
        {
            ProjectId = devtrack.Id,
            Name = "DevTrack.Api",
            Type = ComponentType.Api,
            TechStack = ".NET 10, EF Core, JWT",
            LocalUrl = "http://localhost:8080",
            RepoPath = "src/DevTrack.Api",
            CurrentStatusNote = "Backend bitti — Resume Mode, Dashboard, Quick Capture, soft-delete cascade ve günlük reminder generator yerinde.",
        };
        var dtWeb = new Component
        {
            ProjectId = devtrack.Id,
            Name = "DevTrack.Web",
            Type = ComponentType.WebApp,
            TechStack = "Next.js 16, Tailwind v4, shadcn/ui (Base UI)",
            LocalUrl = "http://localhost:3000",
            RepoPath = "web/",
            CurrentStatusNote = "Prompt 3 frontend bitti. UI biraz daha estetik olabilir — detail panel'in boş hali biraz çıplak duruyor.",
        };
        await _db.Components.AddRangeAsync(new[] { dtApi, dtWeb }, ct);
        await _db.SaveChangesAsync(ct);
        result.Components += 2;

        await _db.ProjectTags.AddRangeAsync(new[]
        {
            new ProjectTag { ProjectId = devtrack.Id, TagId = tagBackend.Id },
            new ProjectTag { ProjectId = devtrack.Id, TagId = tagFrontend.Id },
            new ProjectTag { ProjectId = devtrack.Id, TagId = tagMvp.Id },
        }, ct);

        // ─── Project 4: OldGameProject (Paused, 45 days stale) ────────────────
        var oldGame = new Project
        {
            UserId = userId,
            Name = "Pixel Roguelike",
            Description = "İki ay önce başladığım pixel art roguelike. Multiplayer netcode'da takıldı.",
            Goal = "Bir multiplayer roguelike yapmayı öğrenmek.",
            Status = ProjectStatus.Paused,
        };
        await _db.Projects.AddAsync(oldGame, ct);
        await _db.SaveChangesAsync(ct);
        result.Projects++;

        var oldEngine = new Component
        {
            ProjectId = oldGame.Id,
            Name = "RoguelikeEngine",
            Type = ComponentType.Game,
            TechStack = "Unity, C#",
            CurrentStatusNote = "Lockstep netcode'unda paket sırası sorunu var. Mirror'a geçmeyi düşündüm ama önce burada anlamak istiyorum. Şu an client-side prediction yarıda kaldı.",
        };
        await _db.Components.AddAsync(oldEngine, ct);
        await _db.SaveChangesAsync(ct);
        result.Components++;

        // ─── Project 5: Abandoned (200 days) ──────────────────────────────────
        var abandoned = new Project
        {
            UserId = userId,
            Name = "Eski Portfolyo Sitesi",
            Description = "Üç ay önce başladığım statik portfolyo. Tasarımı beğenmedim, yeni bir tane yapacağım.",
            Status = ProjectStatus.Abandoned,
        };
        await _db.Projects.AddAsync(abandoned, ct);
        await _db.SaveChangesAsync(ct);
        result.Projects++;

        // ─── Learning Track 1: Claude ile Sistem Tasarımı (Active, fresh) ─────
        var sysTrack = new LearningTrack
        {
            UserId = userId,
            Name = "Claude ile Sistem Tasarımı",
            Description = "Distributed sistemler, event sourcing, CQRS, saga.",
            Source = "Claude",
            Status = LearningTrackStatus.Active,
        };
        await _db.LearningTracks.AddAsync(sysTrack, ct);
        await _db.SaveChangesAsync(ct);
        result.LearningTracks++;

        var sysModules = new[]
        {
            new LearningModule { LearningTrackId = sysTrack.Id, Name = "Microservices vs Monolith", Order = 0, Status = LearningModuleStatus.Completed, StartedAt = now.AddDays(-25), CompletedAt = now.AddDays(-20) },
            new LearningModule { LearningTrackId = sysTrack.Id, Name = "Event Sourcing temelleri", Order = 1, Status = LearningModuleStatus.Completed, StartedAt = now.AddDays(-19), CompletedAt = now.AddDays(-12) },
            new LearningModule { LearningTrackId = sysTrack.Id, Name = "CQRS pattern", Order = 2, Status = LearningModuleStatus.InProgress, StartedAt = now.AddDays(-5) },
            new LearningModule { LearningTrackId = sysTrack.Id, Name = "Saga / Orchestration", Order = 3, Status = LearningModuleStatus.NotStarted },
            new LearningModule { LearningTrackId = sysTrack.Id, Name = "Outbox & Inbox patterns", Order = 4, Status = LearningModuleStatus.NotStarted },
        };
        await _db.LearningModules.AddRangeAsync(sysModules, ct);
        await _db.SaveChangesAsync(ct);
        result.LearningModules += sysModules.Length;

        await _db.LearningTrackTags.AddRangeAsync(new[]
        {
            new LearningTrackTag { LearningTrackId = sysTrack.Id, TagId = tagBackend.Id },
            new LearningTrackTag { LearningTrackId = sysTrack.Id, TagId = tagClaude.Id },
        }, ct);

        // ─── Learning Track 2: React 19 (Active, 20 days stale) ───────────────
        var reactTrack = new LearningTrack
        {
            UserId = userId,
            Name = "React 19 — yeniler",
            Description = "Server Components, use(), Form Actions, Suspense.",
            Source = "React docs + bloglar",
            Status = LearningTrackStatus.Active,
        };
        await _db.LearningTracks.AddAsync(reactTrack, ct);
        await _db.SaveChangesAsync(ct);
        result.LearningTracks++;

        var reactModules = new[]
        {
            new LearningModule { LearningTrackId = reactTrack.Id, Name = "Server Components nedir, nasıl render olur", Order = 0, Status = LearningModuleStatus.Completed, StartedAt = now.AddDays(-40), CompletedAt = now.AddDays(-32) },
            new LearningModule { LearningTrackId = reactTrack.Id, Name = "use() hook ve Suspense", Order = 1, Status = LearningModuleStatus.Completed, StartedAt = now.AddDays(-31), CompletedAt = now.AddDays(-22) },
            new LearningModule { LearningTrackId = reactTrack.Id, Name = "Form Actions ve useFormStatus", Order = 2, Status = LearningModuleStatus.NotStarted },
        };
        await _db.LearningModules.AddRangeAsync(reactModules, ct);
        await _db.SaveChangesAsync(ct);
        result.LearningModules += reactModules.Length;

        await _db.LearningTrackTags.AddAsync(new LearningTrackTag { LearningTrackId = reactTrack.Id, TagId = tagFrontend.Id }, ct);

        // ─── Learning Track 3: Algoritmalar (Paused) ──────────────────────────
        var algoTrack = new LearningTrack
        {
            UserId = userId,
            Name = "Algoritma okumaları",
            Description = "CLRS'den bölüm bölüm.",
            Source = "Kitap",
            Status = LearningTrackStatus.Paused,
        };
        await _db.LearningTracks.AddAsync(algoTrack, ct);
        await _db.SaveChangesAsync(ct);
        result.LearningTracks++;

        // ─── Worklogs ─────────────────────────────────────────────────────────
        var worklogs = new List<(Worklog w, DateTime loggedAt)>
        {
            (new Worklog { UserId = userId, ProjectId = devtrack.Id, WhatIDid = "DevTrack frontend'inin sayfa yapılarını bitirdim. UserMenu'de DropdownMenuLabel kullanımı patlıyordu — kaldırıp düz div yaptım.", WhatsLeft = "UI'ın detayına girip ufak refinement'lar — özellikle dashboard'daki kart yoğunluğu fazla." }, now.AddHours(-2)),
            (new Worklog { UserId = userId, ComponentId = dtApi.Id, WhatIDid = "Backend'e CORS ve Scalar AllowAnonymous ekledim. Frontend artık doğrudan API'a konuşuyor.", WhatsLeft = null }, now.AddHours(-3)),
            (new Worklog { UserId = userId, ComponentId = aosApi.Id, WhatIDid = "Login endpoint'ine refresh token mantığını çiziktirmeye başladım. JwtTokenService'e RefreshToken üreten ikinci bir metod eklemek gerekiyor.", WhatsLeft = "Refresh token tablosunu mu yoksa Redis'i mi tercih edeceğim — karar vermem lazım." }, now.AddDays(-1)),
            (new Worklog { UserId = userId, ComponentId = aosWeb.Id, WhatIDid = "Ders kayıt sayfasında prerequisite uyarısı için modal tasarladım. Kullanıcı 'devam et' der ya da geri döner.", WhatsLeft = null }, now.AddDays(-1).AddHours(-3)),
            (new Worklog { UserId = userId, ProjectId = aos.Id, WhatIDid = "Components arasında repo gezerken AOS.Api'de exception handling middleware eksik olduğunu fark ettim. NotFound ve ValidationException için generic handler ekledim.", WhatsLeft = "Domain'e özel ConflictException da ekleyelim." }, now.AddDays(-2)),
            (new Worklog { UserId = userId, ComponentId = foodMobile.Id, WhatIDid = "Barkod tarama akışında race condition vardı — aynı barkodu üst üste taradığında iki kez kaydediliyordu. Debounce ile çözdüm.", WhatsLeft = null }, now.AddDays(-3)),
            (new Worklog { UserId = userId, LearningModuleId = sysModules[2].Id, WhatIDid = "CQRS bölümünde write/read modellerinin ayrılması neden gerek netleşti. Eventual consistency'nin pratik etkilerini örnek üzerinden anladım.", WhatsLeft = "Outbox pattern'ı tam oturmadı, modül 4'te tekrar geleceğim." }, now.AddDays(-5)),
            (new Worklog { UserId = userId, LearningTrackId = sysTrack.Id, WhatIDid = "Event sourcing ile snapshot stratejisi arasındaki ilişki üzerine 1 saatlik soru-cevap.", WhatsLeft = null }, now.AddDays(-6)),
            (new Worklog { UserId = userId, ComponentId = oldEngine.Id, WhatIDid = "Lockstep'te paket sırası bozulduğunda determinism kayboluyor — log eklediğimde bu netleşti.", WhatsLeft = "Reorder buffer mı, yoksa input ID'ye göre sıralayan bir scheduler mı?" }, now.AddDays(-45)),
        };

        foreach (var (w, _) in worklogs)
        {
            w.LoggedAt = now; // will fix below
            await _db.Worklogs.AddAsync(w, ct);
        }
        await _db.SaveChangesAsync(ct);
        result.Worklogs = worklogs.Count;

        foreach (var (w, loggedAt) in worklogs)
        {
            await _db.Worklogs.Where(x => x.Id == w.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.LoggedAt, loggedAt).SetProperty(x => x.CreatedAt, loggedAt), ct);
        }

        // ─── Decision-flavored worklogs (Decision was merged into Worklog) ────
        var decisionWorklogs = new List<(Worklog w, DateTime when)>
        {
            (new Worklog
            {
                UserId = userId,
                ProjectId = aos.Id,
                WhatIDid = "JWT seçildi (session yerine)",
                Reasoning = "AOS.Web'i statik host edebilmek istiyorum, server-side session storage zorunluluk yaratırdı. JWT ile stateless API kalır.",
                Alternatives = "Session + Redis. Daha kolay revoke ama cache layer ekliyor — şimdilik gereksiz.",
            }, now.AddDays(-6)),
            (new Worklog
            {
                UserId = userId,
                ComponentId = aosApi.Id,
                WhatIDid = "EF Core soft-delete + global query filter",
                Reasoning = "Hard delete ile silinen kayıtları geri getirmek imkansız oluyor. IsDeleted kolonu + global filter en az ceremony.",
                Alternatives = "Audit log tablosu — overkill; ayrı 'archived' tablolar — schema duplikasyonu.",
            }, now.AddDays(-15)),
            (new Worklog
            {
                UserId = userId,
                LearningTrackId = sysTrack.Id,
                WhatIDid = "Önce CQRS, sonra Saga",
                Reasoning = "Saga'yı CQRS örnekleri üzerinde tartışmak daha mantıklı çünkü saga'nın komutları zaten bir Command bus'ta dolaşacak.",
                Alternatives = "Direkt saga'ya başla — soyut kalır.",
            }, now.AddDays(-10)),
        };
        foreach (var (w, _) in decisionWorklogs)
        {
            w.LoggedAt = now;
            await _db.Worklogs.AddAsync(w, ct);
        }
        await _db.SaveChangesAsync(ct);
        result.Worklogs += decisionWorklogs.Count;
        foreach (var (w, when) in decisionWorklogs)
        {
            await _db.Worklogs.Where(x => x.Id == w.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.LoggedAt, when).SetProperty(x => x.CreatedAt, when), ct);
        }

        // ─── NextSteps ────────────────────────────────────────────────────────
        var steps = new List<NextStep>
        {
            // AOS project-level
            new() { UserId = userId, ProjectId = aos.Id, Description = "Component'ler arasında shared validator helper'ı çıkar", Priority = NextStepPriority.Medium },
            new() { UserId = userId, ProjectId = aos.Id, Description = "AOS deploy planı — VPS mi managed mi karar ver", Priority = NextStepPriority.Low },
            // AOS components
            new() { UserId = userId, ComponentId = aosApi.Id, Description = "Refresh token endpoint'ini ekle ve test et", Priority = NextStepPriority.High },
            new() { UserId = userId, ComponentId = aosApi.Id, Description = "Password reset akışı — e-mail provider entegrasyonu", Priority = NextStepPriority.High },
            new() { UserId = userId, ComponentId = aosWeb.Id, Description = "Ders kayıt: çakışan saat uyarısı UI", Priority = NextStepPriority.Medium },
            // FoodTracker
            new() { UserId = userId, ProjectId = food.Id, Description = "App icon ve splash screen tasarla", Priority = NextStepPriority.Low },
            new() { UserId = userId, ComponentId = foodMobile.Id, Description = "Offline cache + queued sync", Priority = NextStepPriority.High },
            // DevTrack
            new() { UserId = userId, ProjectId = devtrack.Id, Description = "Detail panel boşken daha az çıplak görünsün", Priority = NextStepPriority.Medium },
            new() { UserId = userId, ComponentId = dtApi.Id, Description = "Search endpoint ekle — Prompt sonrası", Priority = NextStepPriority.Low },
            // Pixel Roguelike
            new() { UserId = userId, ProjectId = oldGame.Id, Description = "Multiplayer netcode için mini POC repo aç", Priority = NextStepPriority.Low },
            new() { UserId = userId, ComponentId = oldEngine.Id, Description = "Lockstep paket sırası: reorder buffer dene", Priority = NextStepPriority.Low },
            // Sistem Tasarımı eğitimi
            new() { UserId = userId, LearningTrackId = sysTrack.Id, Description = "Outbox pattern bittikten sonra DevTrack'e uygula", Priority = NextStepPriority.Medium },
            new() { UserId = userId, LearningModuleId = sysModules[3].Id, Description = "Saga modülüne başla — orchestrator vs choreography farkı", Priority = NextStepPriority.Medium },
            // React 19 eğitimi
            new() { UserId = userId, LearningTrackId = reactTrack.Id, Description = "Form Actions modülüne geri dön", Priority = NextStepPriority.High },
            new() { UserId = userId, LearningModuleId = reactModules[2].Id, Description = "useFormStatus örneklerini elle uygula", Priority = NextStepPriority.Medium },
            // Algoritma eğitimi
            new() { UserId = userId, LearningTrackId = algoTrack.Id, Description = "Hash table bölümüne dön", Priority = NextStepPriority.Low },
            // Tamamlanmış (geçmiş için)
            new() { UserId = userId, ComponentId = aosApi.Id, Description = "FluentValidation'a dair tüm DTO'lar için validator hazır", Priority = NextStepPriority.High, IsCompleted = true, CompletedAt = now.AddDays(-2) },
            new() { UserId = userId, ProjectId = devtrack.Id, Description = "CORS yapılandırmasını ekle", Priority = NextStepPriority.High, IsCompleted = true, CompletedAt = now.AddHours(-2) },
        };
        await _db.NextSteps.AddRangeAsync(steps, ct);
        await _db.SaveChangesAsync(ct);
        result.NextSteps = steps.Count;

        // ─── Ideas ────────────────────────────────────────────────────────────
        var ideas = new List<Idea>
        {
            new() { UserId = userId, ProjectId = aos.Id, Content = "Kayıt sırasında öğrenciye 'önerilen dersler' diye AI bazlı bir öneri", CapturedAt = now.AddDays(-3) },
            new() { UserId = userId, ComponentId = aosApi.Id, Content = "Admin işlemlerine rate limit ve audit log", CapturedAt = now.AddDays(-7) },
            new() { UserId = userId, ProjectId = devtrack.Id, Content = "Worklog'ları haftalık özetleyen bir 'Cuma raporu' e-postası", CapturedAt = now.AddDays(-2) },
            new() { UserId = userId, ProjectId = devtrack.Id, Content = "Cmd+K içinden 'son worklog'a göz at' komutu", CapturedAt = now.AddDays(-1) },
            new() { UserId = userId, ComponentId = foodMobile.Id, Content = "Apple Watch komplikasyonu — kalan kalori widget'ı", CapturedAt = now.AddDays(-4) },
            new() { UserId = userId, LearningTrackId = sysTrack.Id, Content = "CQRS örneğini DevTrack üzerinde uygula — read model olarak elasticsearch", CapturedAt = now.AddDays(-6) },
        };
        await _db.Ideas.AddRangeAsync(ideas, ct);
        await _db.SaveChangesAsync(ct);
        result.Ideas = ideas.Count;
        foreach (var i in ideas)
        {
            await _db.Ideas.Where(x => x.Id == i.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.CreatedAt, i.CapturedAt), ct);
        }

        // ─── Resources ────────────────────────────────────────────────────────
        var resources = new List<Resource>
        {
            new() { UserId = userId, ProjectId = aos.Id, Title = "JWT vs Session — Claude konuşması", Url = "https://claude.ai/chat/example-1", Type = ResourceType.ClaudeChat, Notes = "Stateless API tartışması." },
            new() { UserId = userId, ComponentId = aosApi.Id, Title = "ASP.NET Core JWT Bearer dökümanı", Url = "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn", Type = ResourceType.Documentation },
            new() { UserId = userId, ComponentId = dtApi.Id, Title = "EF Core 10 — global query filters", Url = "https://learn.microsoft.com/en-us/ef/core/querying/filters", Type = ResourceType.Documentation },
            new() { UserId = userId, ProjectId = devtrack.Id, Title = "shadcn/ui Base UI dokümantasyonu", Url = "https://ui.shadcn.com/docs", Type = ResourceType.Documentation },
            new() { UserId = userId, LearningTrackId = sysTrack.Id, Title = "Event Sourcing — Greg Young", Url = "https://www.youtube.com/watch?v=8JKjvY4etTY", Type = ResourceType.Video, Notes = "Klasik konuşma." },
            new() { UserId = userId, LearningTrackId = sysTrack.Id, Title = "CQRS Documents by Martin Fowler", Url = "https://martinfowler.com/bliki/CQRS.html", Type = ResourceType.Article },
            new() { UserId = userId, ComponentId = oldEngine.Id, Title = "Glenn Fiedler — networked physics", Url = "https://gafferongames.com/post/networked_physics_in_virtual_reality/", Type = ResourceType.Article },
            new() { UserId = userId, ComponentId = foodMobile.Id, Title = "Open Food Facts API", Url = "https://world.openfoodfacts.org/data", Type = ResourceType.Documentation },
            new() { UserId = userId, ProjectId = devtrack.Id, Title = "DevTrack repo", Url = "https://github.com/example/devtrack", Type = ResourceType.GitHub },
        };
        foreach (var r in resources)
        {
            r.AddedAt = now.AddDays(-Random.Shared.Next(1, 30));
            await _db.Resources.AddAsync(r, ct);
        }
        await _db.SaveChangesAsync(ct);
        result.Resources = resources.Count;

        // ─── Backdate parents (CreatedAt, LastActivityAt) ─────────────────────
        await BackdateProjectAsync(aos.Id, now.AddDays(-30), now.AddHours(-2), ct);
        await BackdateProjectAsync(food.Id, now.AddDays(-25), now.AddDays(-3), ct);
        await BackdateProjectAsync(devtrack.Id, now.AddDays(-10), now.AddHours(-1), ct);
        await BackdateProjectAsync(oldGame.Id, now.AddDays(-90), now.AddDays(-45), ct);
        await BackdateProjectAsync(abandoned.Id, now.AddDays(-220), now.AddDays(-200), ct);

        await BackdateComponentAsync(aosApi.Id, now.AddDays(-30), now.AddHours(-3), ct);
        await BackdateComponentAsync(aosWeb.Id, now.AddDays(-25), now.AddDays(-1).AddHours(-3), ct);
        await BackdateComponentAsync(foodMobile.Id, now.AddDays(-25), now.AddDays(-3), ct);
        await BackdateComponentAsync(dtApi.Id, now.AddDays(-10), now.AddHours(-3), ct);
        await BackdateComponentAsync(dtWeb.Id, now.AddDays(-10), now.AddHours(-2), ct);
        await BackdateComponentAsync(oldEngine.Id, now.AddDays(-90), now.AddDays(-45), ct);

        await BackdateTrackAsync(sysTrack.Id, now.AddDays(-30), now.AddDays(-5), ct);
        await BackdateTrackAsync(reactTrack.Id, now.AddDays(-50), now.AddDays(-22), ct);
        await BackdateTrackAsync(algoTrack.Id, now.AddDays(-120), now.AddDays(-60), ct);

        await BackdateModuleAsync(sysModules[2].Id, now.AddDays(-25), now.AddDays(-5), ct);
        await BackdateModuleAsync(sysModules[3].Id, now.AddDays(-25), null, ct);
        await BackdateModuleAsync(sysModules[4].Id, now.AddDays(-25), null, ct);

        // ─── Reminders — manuel tetikle ───────────────────────────────────────
        var generated = await _reminderGenerator.GenerateForAllUsersAsync(ct);
        result.Reminders = generated;

        return result;
    }

    public async Task<int> WipeAsync(int userId, CancellationToken ct = default)
    {
        var rows = 0;
        rows += await _db.Worklogs.IgnoreQueryFilters().Where(x => x.UserId == userId).ExecuteDeleteAsync(ct);
        // Ideas FK -> NextSteps; null out first
        await _db.Ideas.IgnoreQueryFilters().Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConvertedNextStepId, (int?)null), ct);
        rows += await _db.NextSteps.IgnoreQueryFilters().Where(x => x.UserId == userId).ExecuteDeleteAsync(ct);
        rows += await _db.Ideas.IgnoreQueryFilters().Where(x => x.UserId == userId).ExecuteDeleteAsync(ct);
        rows += await _db.Resources.IgnoreQueryFilters().Where(x => x.UserId == userId).ExecuteDeleteAsync(ct);
        rows += await _db.Reminders.IgnoreQueryFilters().Where(x => x.UserId == userId).ExecuteDeleteAsync(ct);

        // Junctions for this user's tags / projects / components / tracks / modules
        var userProjectIds = await _db.Projects.IgnoreQueryFilters().Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync(ct);
        var userTrackIds = await _db.LearningTracks.IgnoreQueryFilters().Where(t => t.UserId == userId).Select(t => t.Id).ToListAsync(ct);
        var userComponentIds = await _db.Components.IgnoreQueryFilters().Where(c => userProjectIds.Contains(c.ProjectId)).Select(c => c.Id).ToListAsync(ct);

        rows += await _db.ProjectTags.Where(pt => userProjectIds.Contains(pt.ProjectId)).ExecuteDeleteAsync(ct);
        rows += await _db.ComponentTags.Where(ct2 => userComponentIds.Contains(ct2.ComponentId)).ExecuteDeleteAsync(ct);
        rows += await _db.LearningTrackTags.Where(lt => userTrackIds.Contains(lt.LearningTrackId)).ExecuteDeleteAsync(ct);

        rows += await _db.LearningModules.IgnoreQueryFilters().Where(m => userTrackIds.Contains(m.LearningTrackId)).ExecuteDeleteAsync(ct);
        rows += await _db.Components.IgnoreQueryFilters().Where(c => userProjectIds.Contains(c.ProjectId)).ExecuteDeleteAsync(ct);
        rows += await _db.Projects.IgnoreQueryFilters().Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
        rows += await _db.LearningTracks.IgnoreQueryFilters().Where(t => t.UserId == userId).ExecuteDeleteAsync(ct);
        rows += await _db.Tags.IgnoreQueryFilters().Where(t => t.UserId == userId).ExecuteDeleteAsync(ct);

        return rows;
    }

    private Task BackdateProjectAsync(int id, DateTime createdAt, DateTime? lastActivity, CancellationToken ct)
        => _db.Projects.IgnoreQueryFilters().Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.CreatedAt, createdAt)
                .SetProperty(p => p.LastActivityAt, lastActivity), ct);

    private Task BackdateComponentAsync(int id, DateTime createdAt, DateTime? lastActivity, CancellationToken ct)
        => _db.Components.IgnoreQueryFilters().Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.CreatedAt, createdAt)
                .SetProperty(c => c.LastActivityAt, lastActivity), ct);

    private Task BackdateTrackAsync(int id, DateTime createdAt, DateTime? lastActivity, CancellationToken ct)
        => _db.LearningTracks.IgnoreQueryFilters().Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.CreatedAt, createdAt)
                .SetProperty(t => t.LastActivityAt, lastActivity), ct);

    private Task BackdateModuleAsync(int id, DateTime createdAt, DateTime? lastActivity, CancellationToken ct)
        => _db.LearningModules.IgnoreQueryFilters().Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.CreatedAt, createdAt)
                .SetProperty(m => m.LastActivityAt, lastActivity), ct);
}

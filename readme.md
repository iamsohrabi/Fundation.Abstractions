# Fundation.Abstractions

کتابخانه‌ای از قراردادها و نوع‌های پایه برای ساخت سرویس‌های .NET با الگوهای دامنه‌محور، CQRS، پیام‌رسانی و لایه‌های persistence. این پروژه عمدتاً **abstraction** ارائه می‌کند: interfaceها، typeهای انتقالی و قراردادهای مورد استفاده بین لایه‌ها. پیاده‌سازی cache، broker، scheduler، repository، event store و ثبت سرویس‌ها در DI جزو این پروژه نیست و باید در برنامه یا کتابخانه زیرساختی مصرف‌کننده فراهم شود.

## فهرست مطالب

- [نیازمندی‌ها](#نیازمندیها)
- [استفاده در پروژه](#استفاده-در-پروژه)
- [ساختار کتابخانه](#ساختار-کتابخانه)
- [دامنه و هویت](#دامنه-و-هویت)
- [CQRS](#cqrs)
- [رویدادهای دامنه و event sourcing](#رویدادهای-دامنه-و-event-sourcing)
- [Persistence و تراکنش](#persistence-و-تراکنش)
- [Cache](#cache)
- [پیام‌رسانی و Outbox/Inbox](#پیامرسانی-و-outboxinbox)
- [زمان‌بندی](#زمانبندی)
- [Serialization](#serialization)
- [Web و ماژول‌ها](#web-و-ماژولها)
- [سایر قراردادها](#سایر-قراردادها)
- [وابستگی‌های مستقیم](#وابستگیهای-مستقیم)
- [نکته‌های طراحی و محدودیت‌ها](#نکتههای-طراحی-و-محدودیتها)

## نیازمندی‌ها

- .NET SDK 10 یا جدیدتر
- Target Framework کتابخانه: `net10.0`
- ارجاع framework به `Microsoft.AspNetCore.App`؛ مصرف‌کننده‌هایی که framework referenceهای پروژه را به ارث نمی‌برند باید نیازهای ASP.NET Core را در پروژه خود بررسی کنند.

## استفاده در پروژه

برای استفاده از پروژه در یک solution، یک `ProjectReference` به فایل پروژه اضافه کنید. برای نمونه، مسیر نسبی را متناسب با ساختار solution خود تنظیم کنید:

```xml
<ItemGroup>
  <ProjectReference Include="..\Fundation.Abstractions\Fundation.Abstractions.csproj" />
</ItemGroup>
```

سپس restore و build کنید:

```bash
dotnet restore
dotnet build
```

فضای نام اصلی کتابخانه `Fundation.Abstractions` است و namespaceهای تخصصی در بخش‌های زیر آمده‌اند. پروژه handlerها یا سرویس‌های concrete را به‌صورت خودکار کشف یا در DI ثبت نمی‌کند؛ این کار به پیاده‌سازی و composition root برنامه وابسته است.

## ساختار کتابخانه

| مسیر | مسئولیت اصلی |
|---|---|
| `Core` | قفل انحصاری و تولید شناسه |
| `Domain` | هویت، entity، aggregate، audit و business rule |
| `Domain/EventSourcing` | قرارداد aggregate مبتنی بر event sourcing و نسخه آن |
| `CQRS/Command` | commandها، handlerها و پردازش command |
| `CQRS/Query` | queryها، handlerها، صفحه‌بندی و query جریانی |
| `CQRS/Event` | eventهای عمومی، handler، mapping و dispatch |
| `CQRS/Event/Internal` | قراردادهای داخلی domain event و notification event |
| `Caching` | دسترسی به cache و policyهای cache/invalidation |
| `Persistence` | repository عمومی، unit of work، تراکنش و seeding |
| `Persistence/EfCore` | قراردادهای اختصاصی Entity Framework Core |
| `Persistence/Mongo` | قراردادهای اختصاصی MongoDB |
| `Persistence/EventStore` | stream event، event store، aggregate store و projection |
| `Messaging` | bus، message، context، acknowledgement و قراردادهای مصرف/انتشار |
| `Messaging/PersistMessage` | نوع‌ها و سرویس ذخیره‌سازی پیام‌های ورودی، خروجی و داخلی |
| `Scheduling` | زمان‌بندی commandها و درخواست‌ها |
| `Serialization` | serialization عمومی و پیام‌ها |
| `Types` | فهرست typeها و اطلاعات instance |
| `Web` | پردازش gateway، module، endpointهای Minimal API و storage درخواست |

## دامنه و هویت

قراردادهای دامنه در `Fundation.Abstractions.Domain` قرار دارند:

- `IIdentity<TId>` مقدار شناسه را از طریق `Value` عرضه می‌کند. `Identity<TId>` نوع record پایه برای شناسه‌های value-object است.
- `EntityId<T>` و `AggregateId<T>` wrapperهای عمومی شناسه هستند. نوع‌های غیر generic آن‌ها بر پایه `long` ساخته شده‌اند؛ سازنده `AggregateId(long)` مقدار صفر و منفی را رد می‌کند.
- `IHaveIdentity<TId>` و `IEntity<TId>` قرارداد شناسه و اطلاعات ایجادکننده را فراهم می‌کنند. `IEntity` ساده از `EntityId` استفاده می‌کند.
- `IAggregate<TId>` علاوه بر entity، قرارداد `IHaveAggregate` را پیاده می‌کند. aggregate مسئول نگهداری و پاک‌سازی domain eventهای تأییدنشده و بررسی `IBusinessRule` است.
- `IHaveAggregateVersion.OriginalVersion` نسخه‌ای است که هنگام بارگذاری از ذخیره‌ساز دریافت شده و برای کنترل optimistic concurrency به کار می‌رود.
- `IHaveCreator` شامل `Created` و `CreatedBy` است؛ `IHaveAudit` اطلاعات آخرین تغییر را نیز اضافه می‌کند.
- `IHaveSoftDelete` در حال حاضر marker interface خالی است؛ این قرارداد property یا رفتار حذف نرم را تعریف نمی‌کند.

یک قاعده کسب‌وکار با `IBusinessRule` دو عضو دارد: `IsBroken()` و متن `Message`. این interface نحوه تبدیل نقض قاعده به exception یا پاسخ HTTP را تعیین نمی‌کند.

## CQRS

قراردادها در `Fundation.Abstractions.CQRS.Command` و `Fundation.Abstractions.CQRS.Query` هستند و بر پایه request/notificationهای MediatR ساخته شده‌اند.

### Command

- `ICommand<TResult>` command دارای خروجی و `ICommand` شکل بدون خروجی آن است.
- `ICommandHandler<TCommand, TResult>` و `ICommandHandler<TCommand>` قرارداد handler هستند.
- `ICreateCommand`، `IUpdateCommand` و `IDeleteCommand` markerهای معنایی command هستند؛ به‌تنهایی منطق ذخیره‌سازی یا validation اضافه نمی‌کنند.
- `ITxRequest` یک marker برای درخواست‌هایی است که زیرساخت مصرف‌کننده باید به‌صورت transactional اجرا کند. گونه‌های `ITxCommand`، `ITxCreateCommand`، `ITxUpdateCommand` و `ITxInternalCommand` این marker را ترکیب می‌کنند.
- `IInternalCommand` دارای `Id`، `OccurredOn` و `Type` است و از `ICommand` مشتق می‌شود.
- `ICommandProcessor` ارسال command و زمان‌بندی یک یا چند internal command را تعریف می‌کند.

### Query

- `IQuery<TResult>` برای query با پاسخ و `IStreamQuery<TResult>` برای پاسخ جریانی است.
- `IQueryHandler<TQuery, TResult>` و `IStreamQueryHandler<TQuery, TResult>` قرارداد handlerهای متناظر هستند.
- `IQueryProcessor` پاسخ معمولی را به‌شکل `Task<TResult>` و پاسخ جریانی را به‌شکل `IAsyncEnumerable<TResult>` ارسال می‌کند.
- `IItemQuery<TId, TResult>` شناسه و فهرست `Includes` را تعریف می‌کند.
- `IListQuery<TResult>` از `IPageRequest` پیروی می‌کند. `IPageRequest` propertyهای `Page`، `PageSize`، `Includes`، `Filters` و `Sorts` را تعریف می‌کند؛ مقدار پیش‌فرض یا نحو تفسیر فیلترها در این قرارداد مشخص نشده است.
- `FilterModel` یک record با `FieldName`، `Comparision` و `FieldValue` است. نام property در API فعلی دقیقاً `Comparision` است.

نمونه query و handler:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Fundation.Abstractions.CQRS.Query;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderDto>;

public sealed record OrderDto(Guid Id, string Number);

public sealed class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
	public Task<OrderDto> Handle(
		GetOrderQuery request,
		CancellationToken cancellationToken)
	{
		var result = new OrderDto(request.OrderId, "ORD-001");
		return Task.FromResult(result);
	}
}
```

نمونه صرفاً شکل قراردادها را نشان می‌دهد؛ handler بالا برای ساده ماندن مثال داده واقعی را از persistence نمی‌خواند.

## رویدادهای دامنه و event sourcing

`IEvent` در `Fundation.Abstractions.CQRS.Event` یک MediatR notification است و شناسه، نوع، زمان وقوع، timestamp و نسخه رویداد را تعریف می‌کند. `IEventHandler<TEvent>` قرارداد handler آن است.

سه نوع رویداد را از هم جدا کنید:

1. `IDomainEvent` در namespace داخلی `Fundation.Abstractions.CQRS.Event.Internal` قرار دارد؛ علاوه بر مشخصات `IEvent`، شناسه aggregate و شماره توالی aggregate را دارد.
2. `IDomainNotificationEvent` برای notification دامنه‌ای است که از domain event نگاشت می‌شود.
3. `IIntegrationEvent` یک `IMessage` است و برای مرز پیام‌رسانی خارجی به کار می‌رود.

`IEventMapper` قرارداد نگاشت domain eventها به notificationهای داخلی و integration eventها را فراهم می‌کند. `IEventProcessor` دو مسیر جدا دارد: `DispatchAsync` برای dispatch داخلی handlerها و `PublishAsync` برای ارسال به مسیر ذخیره/انتشار. توضیح interface، مسیر publish را به‌صورت outbox و تحویل at-least-once توصیف می‌کند؛ تحقق تضمین نهایی به پیاده‌سازی concrete وابسته است.

`IDomainEventsAccessor` فهرست eventهای commit‌نشده را در دسترس می‌گذارد. `IAggregatesDomainEventsRequestStore` eventها را از aggregateها جمع‌آوری می‌کند؛ `IDomainEventContext` علاوه بر خواندن eventهای جمع‌شده، علامت‌گذاری آن‌ها به‌عنوان commit‌شده را تعریف می‌کند.

### Event sourcing

- `IEventSourcedAggregate<TId>` ترکیبی از entity و `IHaveEventSourcingAggregate` است.
- `IHaveEventSourcedAggregateVersion` مقدارهای `OriginalVersion` و `CurrentVersion` را در دسترس می‌گذارد.
- `IHaveEventSourcingAggregate.LoadFromHistory` برای بازسازی از domain eventهای history تعریف شده است.
- `IEventStore` خواندن stream، بررسی وجود stream، append یک یا چند event، بازسازی aggregate و commit را تعریف می‌کند.
- `IStreamEvent` پوششی برای `IDomainEvent` و metadata آن است.
- `IAggregateStore` بارگذاری، ذخیره و بررسی وجود aggregate event-sourced را فراهم می‌کند.
- `ExpectedStreamVersion.NoStream` برابر `-1` و `ExpectedStreamVersion.Any` برابر `-2` است. `StreamReadPosition.Start` برابر صفر است. تفسیر سایر مقدارها باید با پیاده‌سازی event store هماهنگ باشد.
- `AppendResult` موقعیت سراسری و نسخه مورد انتظار بعدی را گزارش می‌کند.
- `IHaveAggregateStateProjection.When` و `Fold` برای دو مسیر اعمال/بازسازی state تعریف شده‌اند؛ پیاده‌سازی aggregate تعیین می‌کند هر event دقیقاً چگونه state را تغییر دهد.
- طبق قرارداد فعلی، `When` event را بدون افزایش نسخه اعمال می‌کند؛ `Fold` برای بازسازی از history، نسخه جاری و آخرین نسخه commit را جلو می‌برد.
- `IStreamEventMetadata` شناسه رویداد، موقعیت اختیاری در log و موقعیت درون stream را نگه می‌دارد.
- `IReadProjectionPublisher` و `IHaveReadProjection` قرارداد انتشار event به read projection را تعریف می‌کنند.
- `IAggregatesDomainEventsRequestStore` eventهای تأییدنشده را از aggregateها جمع می‌کند و در اختیار پردازش request قرار می‌دهد.

## Persistence و تراکنش

### Repository و unit of work عمومی

`IReadRepository<TEntity, TId>` عملیات خواندن با شناسه یا predicate و دریافت فهرست را تعریف می‌کند. `IWriteRepository<TEntity, TId>` افزودن، به‌روزرسانی و حذف را تعریف می‌کند. هر دو نیاز دارند که entity، `IHaveIdentity<TId>` را پیاده کند. `IRepository<TEntity, TId>` این دو را ترکیب می‌کند و `IDisposable` است؛ گونه‌ی تک پارامتری از شناسه‌ی `long` استفاده می‌کند.

`IUnitOfWork` عملیات شروع تراکنش و commit را تعریف می‌کند و `IUnitOfWork<TContext>` context را ارائه می‌دهد. قرارداد عمومی unit of work متد rollback ندارد؛ rollback در `ITransactionAble` تعریف شده است. `ITxDbContextExecution` اجرای delegate در تراکنش و `IRetryDbContextExecution` اجرای مجدد عملیات را تعریف می‌کنند، اما تعداد تلاش، policy و رفتار خطا را مشخص نمی‌کنند.

`IDataSeeder.SeedAllAsync()` قرارداد seeding است. هیچ seeder concrete همراه این package تعریف نشده است.

### Entity Framework Core

در `Fundation.Abstractions.Persistence.EfCore`:

- `IDbContext` شامل `DbSet<TEntity>`، ذخیره تغییرات و عملیات تراکنش با `IsolationLevel` است.
- `IEfUnitOfWork` قراردادهای unit of work، تراکنش و اجرای transactional/retry را ترکیب می‌کند. گونه‌ی generic، `DbContext` مشخص را ارائه می‌دهد.
- `IEfRepository<TEntity, TId>` repository عمومی را با queryهای دارای `Include` گسترش می‌دهد. overloadهای predicate امکان انتخاب tracking را هم دارند.
- `IPageRepository<TEntity, TKey>` در نسخه فعلی فقط یک marker خالی است و API صفحه‌بندی تعریف نمی‌کند.
- `IConnectionFactory` یک `IDbConnection` را ایجاد یا بازمی‌گرداند.
- `IDbFacadeResolver` دسترسی به `DatabaseFacade` را فراهم می‌کند.

### MongoDB

در `Fundation.Abstractions.Persistence.Mongo`، `IMongoDbContext` دسترسی به collection، ذخیره تغییرات و عملیات تراکنش را تعریف می‌کند و امکان ثبت commandهای asynchronous را دارد. `IMongoRepository<TEntity, TId>` همان قرارداد repository عمومی را برای entityهای دارای identity به کار می‌گیرد. قابلیت واقعی transaction به پیکربندی و پیاده‌سازی MongoDB وابسته است.

## Cache

در `Fundation.Abstractions.Caching`:

- `ICacheProvider` عملیات پایه get/set، بررسی وجود و حذف را دارد.
- `ICacheManager` API سطح بالاتر synchronous و asynchronous، مقداردهی از طریق factory و `GetOrSet` را ارائه می‌دهد.
- `cacheTime` در توضیح API بر حسب ثانیه است؛ معنای مقدار `null` به پیاده‌سازی provider واگذار شده است.
- `ICachePolicy<TRequest, TResponse>` و `IStreamCachePolicy<TRequest, TResponse>` زمان انقضا و cache key را برای requestهای MediatR تعریف می‌کنند.
- `IInvalidateCachePolicy` کلیدهایی را مشخص می‌کند که باید هنگام اجرای request پاک شوند.

پیاده‌سازی پیش‌فرض `GetCacheKey` نام کامل نوع request و مقادیر propertyهای آن را با reflection کنار هم می‌گذارد. برای کنترل نسخه‌بندی کلید، حذف داده حساس، یا جلوگیری از برخورد کلیدها، آن را در policy اختصاصی override کنید.

## پیام‌رسانی و Outbox/Inbox

namespace اصلی این بخش `Fundation.Abstractions.Messaging` است:

- `IMessage` شناسه و زمان ایجاد پیام را تعریف می‌کند؛ `IIntegrationEvent` نوع پیام مورد استفاده برای integration event است.
- `IMessageHandler<TMessage>` متد asynchronous با `IConsumeContext<TMessage>` و `CancellationToken` دارد.
- `IConsumeContext` پیام، headerها، message id/type، زمان ایجاد، اندازه payload، version، `ActivityContext` و `ContextItems` را ارائه می‌دهد.
- `ContextItems` داده‌های key/value را در طول پردازش نگه می‌دارد؛ `AddItem` فقط وقتی کلید وجود ندارد مقدار را اضافه می‌کند و `TryGetItem<T>` در نبود مقدار یا ناسازگاری نوع، مقدار پیش‌فرض `T` را برمی‌گرداند.
- `IBusProducer` انتشار پیام با header و مقصد اختیاری را تعریف می‌کند. `IBusConsumer` شکل‌های مختلف ثبت مصرف‌کننده را دارد و `IBus` علاوه بر آن‌ها start/stop را اضافه می‌کند.
- `MessageHandler<TMessage>` و `MessageHandlerAck<TMessage>` delegateهای جایگزین handler هستند.
- `Acknowledgement` سه نتیجه‌ی `Ack`، `Nack` و `Reject` را مدل می‌کند. در `Nack` و `Reject` مقدار پیش‌فرض `Requeue` برابر `true` است؛ معنی دقیق این نتیجه برای broker تابع adapter است.
- `MessageEnvelope` پیام را همراه dictionary مربوط به headerها نگه می‌دارد؛ `MessageEnvelope<TMessage>` نسخه‌ی strongly typed برای `IMessage` است.
- `IConsumeConfigurationBuilder` در حال حاضر interface خالی است؛ جزئیات پیکربندی مصرف‌کننده در abstraction دیگری تعریف نشده‌اند.

در `Fundation.Abstractions.Messaging.PersistMessage`:

- `MessageDeliveryType` یک enum از نوع flags با مقدارهای `Outbox = 1`، `Inbox = 2` و `Internal = 4` است.
- `MessageStatus` مقدارهای `Stored = 1` و `Processed = 2` را دارد.
- `StoreMessage` payload سریال‌شده، نوع داده، مقصد/نوع delivery، زمان ایجاد، وضعیت و تعداد retry را نگه می‌دارد.
- `IMessagePersistenceService` ذخیره پیام publish/receive/internal و notification، پردازش یک پیام یا همه پیام‌ها را تعریف می‌کند.
- `IMessagePersistenceRepository` عملیات CRUD و query روی `StoreMessage` را تعریف می‌کند.

این قراردادها به‌تنهایی atomicity بین پایگاه داده و broker، ترتیب پیام‌ها یا سیاست retry را تضمین نمی‌کنند؛ این تضمین‌ها باید در پیاده‌سازی و پیکربندی انتخابی مشخص شوند.

## زمان‌بندی

`IScheduler` enqueue فوری، schedule در `DateTimeOffset` معین و schedule تکرارشونده بر اساس cron expression را برای requestهای MediatR تعریف می‌کند. `IScheduleExecutor` اجرای request را انتزاع می‌کند و `ICommandScheduler` برای internal commandها API جداگانه دارد. قالب cron، persistence زمان‌بندی، timezone و رفتار retry در این قراردادها تعیین نشده‌اند.

`ScheduleSerializedObject` حاوی نام کامل type، assembly، داده و توضیح اضافی است. با وجود قرارگیری فایل در پوشه `Scheduling`، namespace این type در کد فعلی `Fundation.Abstractions.Scheduler` (مفرد) است.

## Serialization

`ISerializer` قرارداد serialize/deserialize نوع‌دار و runtime را تعریف می‌کند و گزینه‌های `camelCase` و `indented` دارد. `IMessageSerializer` این API را برای `MessageEnvelope` و `IMessage` گسترش می‌دهد و deserialize از رشته یا `ReadOnlySpan<byte>` با نوع payload را فراهم می‌کند. فرمت wire، سیاست polymorphism و تنظیمات سازگاری نسخه باید در serializer concrete تعریف شوند.

## Web و ماژول‌ها

- `IModuleDefinition` سه مرحله/مسئولیت را قرارداد می‌کند: ثبت سرویس‌ها، پیکربندی pipeline و نگاشت endpointها.
- `IMinimalEndpointDefinition` یک endpoint definition را روی `IEndpointRouteBuilder` نگاشت می‌کند.
- `ICompositionRoot` دسترسی به `IServiceProvider` و تعریف module و ایجاد scope را ارائه می‌دهد.
- `IGatewayProcessor<TModule>` اجرای delegate در scope، ارسال command/query، اجرای کارهای وابسته به processor و انتشار از طریق `IBus` را در مرز web فراهم می‌کند. برخی overloadها `IMapper` را نیز در اختیار delegate می‌گذارند.
- `IRequestStorage` فضای ذخیره‌سازی ساده‌ی key/value برای طول عمری که پیاده‌سازی تعیین می‌کند تعریف می‌کند؛ lifetime آن در این interface مشخص نیست.

برای `SendQueryAsync<TResponse>`، پاسخ باید class باشد؛ overloadهای `ExecuteQuery` constraint متفاوتی دارند. در پیاده‌سازی gateway، تفاوت scope و lifetime وابستگی‌ها را صریح مدیریت کنید.

## سایر قراردادها

- `IIdGenerator<TId>` تولید شناسه جدید را انتزاع می‌کند.
- `IExclusiveLock` عملیات acquire/release و اجرای delegate همگام یا asynchronous را عرضه می‌کند. سیاست fairness، reentrancy و timeout در قرارداد مشخص نشده است.
- `ITypeList<TBaseType>` فهرستی از `Type`ها با متدهای strongly typed برای افزودن، بررسی، حذف و افزودن یکتا است.
- `IMachineInstanceInfo` گروه کلاینت و شناسه‌ی instance را ارائه می‌دهد.
- `IEventRepository<TContext, TEvent>` یک قرارداد repository تخصصی برای eventها است.

## وابستگی‌های مستقیم

نسخه‌های زیر در فایل پروژه pin شده‌اند. وابستگی transitive ممکن است نسخه‌های دیگری به graph اضافه کند.

| Package | Version |
|---|---:|
| `Microsoft.DotNet.PlatformAbstractions` | `3.1.6` |
| `Microsoft.EntityFrameworkCore` | `10.0.0` |
| `Microsoft.EntityFrameworkCore.Abstractions` | `10.0.0` |
| `Microsoft.Extensions.Configuration.Abstractions` | `10.0.0` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | `10.0.0` |
| `Microsoft.Extensions.Hosting.Abstractions` | `10.0.0` |
| `Microsoft.Extensions.Logging.Abstractions` | `10.0.0` |
| `Microsoft.Extensions.Options` | `10.0.0` |
| `AutoMapper` | `16.2.0` |
| `Ardalis.GuardClauses` | `4.6.0` |
| `MediatR.Extensions.Microsoft.DependencyInjection` | `11.1.0` |
| `MongoDB.Driver` | `3.8.1` |

علاوه بر این packageها، پروژه به framework reference با نام `Microsoft.AspNetCore.App` نیاز دارد. این کتابخانه یک abstraction package است و انتخاب provider یا نسخه‌ی driverهای دیگر، وظیفه‌ی پیاده‌سازی زیرساخت مصرف‌کننده است.

## نکته‌های طراحی و محدودیت‌ها

1. همه‌ی قراردادهای موجود در یک سرویس الزاماً با هم استفاده نمی‌شوند؛ فقط abstractionهای مورد نیاز را پیاده‌سازی و ثبت کنید.
2. `ITxRequest`، `IHaveSoftDelete` و `IPageRepository` به‌تنهایی رفتار اجرایی پیاده نمی‌کنند؛ marker بودن آن‌ها را با قابلیت آماده اشتباه نگیرید.
3. قراردادهای repository و unit of work رفتار providerها را یکسان نمی‌کنند. ترجمه‌ی expressionها، tracking، transaction و disposal تابع adapter است.
4. متدهای این پروژه عمدتاً asynchronous هستند، اما همه‌ی APIها `CancellationToken` ندارند؛ فقط در جاهایی که در امضای متد آمده، cancellation را می‌توان از این abstraction عبور داد.
5. نسخه‌های package و Target Framework در خود `Fundation.Abstractions.csproj` تعریف شده‌اند. تغییر نسخه‌ی یک dependency ممکن است روی مصرف‌کننده‌ها و پیاده‌سازی‌های concrete اثر بگذارد؛ ارتقا را همراه با build و آزمون integration همان provider بررسی کنید.


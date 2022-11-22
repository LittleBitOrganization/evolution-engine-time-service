# Time service

Сервис дает доступ к *TimeTweener* и *ITimeService*.

**TimeTweener** - класс для удобной работы с таймерами. Помогает запускать таймер, пропускать, сохранять и работает даже при выходе из игры.

**ITimeService** - дает доступ к реальному времени и моментам открытия, закрытия приложения.


### Импорт

```json
"com.littlebitgames.timeservicemodule": "https://github.com/LittleBitOrganization/evolution-engine-time-service.git"
```


### Зависимости

[Кор модуль](https://github.com/LittleBitOrganization/evolution-engine-core) - для доступа к IDataStorageService и ILifecycle.

**Импорт кор модуля:**
```json
"com.littlebitgames.coremodule": "https://github.com/LittleBitOrganization/evolution-engine-core.git"
```


### Инсталлеры

```ruby
            Container
                .Bind<FirstPlayChecker>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<ITimeService>()
                .To<TimeService>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<TimeTweenerFactory>()
                .AsSingle()
                .NonLazy();
```
### Примеры кода 

```ruby
_tweener = _timeTweenerFactory.Create(_cooldown.TimeStart, _cooldown.Duration);
_tweener = _timeTweenerFactory.CreateFromNow(cooldown.Duration);
if (_tweener.IsCompleted() == false)
{
  Debug.Log(_tweener.GetCurrentTime());
}

```


### To Implement
- Some way to generate:
  - ```c#
    foreach (ctor)
    {   
        `T ..ctor(cParams)`
        emit:
        T? New(cParams?)
    }    
	```

# IMPL

A [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) that creates implementations!

You create as much of the base implementation as you want and then `IMPL` does the rest. Simply mark your existing implementation with `[Implement]` .



## `ImplementAttribute`

- `IMPL.Contracts.ImplementAttribute`
- Can be applied to `interface`, `class`, and `struct` declarations.
- Has optional Properties/Parameters to control the implementation
  - `Name`: Controls the name of the implementation for an `interface`, has no effect on other declarations.
  - `Declaration`: This `string` can contain any number of declaration `keywords` that control how 

## `IMPL.Contracts`

#### `ImplementAttribute`

- Can be applied to any member declaration.
- `Declaration`: This `string` can contain any number of declaration `keywords` that will apply to the implemented member
  - Visibility
    - `private`, `protected`, `internal`, `public`

  - Instancing
    - `instance`, `static` -- Yes, you make an `interface` into a `static` implementation!

  - Others
    - `sealed`, `abstract`, `virtual`, `override`
    - `new`, `readonly`, `partial`


#### `ImplementTypeAttribute : ImplementAttribute`

- `Name`: Controls the name of the implementation for an `interface`, has no effect on other declarations.

#### `DisposeAttribute`

- When applied to a `Field` or `Property`, said member will be disposed when `Dispose()` is called



## Implementable Interfaces

- `IDisposable`
  - default: `Dispose()` will cause all `events` to be set to `null`
  - `fields` and `properties` can also be disposed by adding `DisposeAttribute` to them
- `INotifyPropertyChanged` + `INotifyPropertyChanging`
  
  -
  
  ```c#
  private {Type} _{fieldName};
  
  public {Type} {PropertyName}
  {
      get => this._{fieldName};
      set => this.SetField<{Type}>(ref _{fieldName}, value);
  }
  
  public event PropertyChangingEventHandler? PropertyChanging;
  public event PropertyChangedEventHandler? PropertyChanged;
  
  private|protected bool SetField<T>(ref T field, T value, bool force = false, [CallerMemberName] string? propertyName = null)
  {
      if (force || !EqualityComparer<T>.Default.Equals(field, value))
      {
          this.OnPropertyChanging(propertyName);
          field = value;
          this.OnPropertyChanged(propertyName);
          return true;
      }
      return false;
  }
  
  private|protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
  {
      this.PropertyChanging?.Invoke(sender: this, args: new PropertyChangingEventARgs(propertyName));
  }
  
  private|protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
      this.PropertyChanged?.Invoke(sender: this, args: new PropertyChangedEventARgs(propertyName));
  }
  ```
  
  





## Implementation Details

- nullability
  - `default`: All emitted code files have `#nullable enable` at the top and all relevant nullable annotations will be included.

- `Events`

  - `EventHandler`
  	```c#
    public void event EventHandler? {EventName};
    
    private|protected void On{EventName}()
    {
        this.{EventName}?.Invoke(sender: this, args: EventArgs.Empty);
    }
    ```
  - `EventHandler<TArgs>`
    ```c#
    public void event EventHandler<TArgs>? {EventName};
    
    private|protected void On{EventName}(EventArgs<TArgs> args)
    {
        this.{EventName}?.Invoke(sender: this, args: args);
    }
    
    private|protected void On{EventName}(TArgs args)
    {
        this.{EventName}?.Invoke(sender: this, args: new EventArgs<TArgs>(args));
    }
    ```
  
  
  - 
  

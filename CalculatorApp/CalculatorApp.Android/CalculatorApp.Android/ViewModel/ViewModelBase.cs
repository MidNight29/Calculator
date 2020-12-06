
namespace CalculatorXamarinApp.ViewModel
{
    using CalculatorXamarinApp.Interfaces;
    using MvvmCross.Commands;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using static CalculatorXamarinApp.ViewModel.DomainObjectPropertyAttribute;

    // TODO: move attribute implementation to separate class file
    /// <summary>
    /// The <see cref="DomainObjectPropertyAttribute"/> class is used to decorate properties of the ViewModel as they relate to the domain Model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DomainObjectPropertyAttribute : Attribute
    {
        public enum Conversion
        {
            None,
            All,
            ToDomain,
            FromDomain
        }

        /// <summary>
        /// The <see cref="System.Reflection.MemberInfo.Name"/> of the Property in the Model that this Property maps to.
        /// </summary>
        public string Name { get; set; }
        public string PropertyOf { get; set; }
        public Conversion Skip { get; set; }
        public Type ConverterType { get; set; }
        public Type ObjectType { get; set; }

        public DomainObjectPropertyAttribute(string name = null)
        {
            Name = name;
        }
    }

    public abstract class ViewModelBase : MvxViewModel, IMvxViewModel
    {
        protected readonly IMvxNavigationService NavigationService;

        protected ViewModelBase(IMvxNavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        /// <summary>
        /// Returns an object of type <typeparamref name="T"/> with values from this <see cref="ViewModelBase"/> instance.
        /// </summary>
        /// <typeparam name="T">The domain Model.</typeparam>
        /// <param name="target">An optional instance of the domain Model to update the Property values of.</param>
        /// <returns>An object of type <typeparamref name="T"/> with values from this ViewModel instance.</returns>
        protected T ToDomainObject<T>(T target = null, bool strict = false) where T : class, new()
        {
            var obj = target ?? new T();

            var sourceType = GetType();
            var targetType = typeof(T);

            var sourceProperties = sourceType.GetProperties()
                .Where(property => property.CanRead)
                .Select(property => new { property.Name, Attr = property.GetCustomAttribute<DomainObjectPropertyAttribute>() })
                .Where(property => property.Attr?.Skip != Conversion.All && property.Attr?.Skip != Conversion.ToDomain && ((property.Attr?.ObjectType == null && !strict) || property.Attr?.ObjectType == targetType));

            // TODO: add nested sub-properties (e.g., Location.Address.Owner.Role)
            var children = sourceProperties
                .Where(property => !string.IsNullOrWhiteSpace(property.Attr?.PropertyOf))
                .Select(property => property.Attr.PropertyOf)
                .Distinct()
                .ToDictionary(propertyName => propertyName, propertyName =>
                {
                    return targetType.GetProperty(propertyName)?.PropertyType is Type childPropertyType ? (childPropertyType, Activator.CreateInstance(childPropertyType)) :
                        throw new ArgumentException($"Failed to infer Type of Property '{propertyName}' for '{targetType.GetType()}'");
                });

            foreach (var property in sourceProperties)
            {
                if (sourceType.GetProperty(property.Name)?.GetValue(this) is object sourceValue)
                {
                    if (typeof(IDomainObjectConverter).IsAssignableFrom(property.Attr?.ConverterType))
                    {
                        var converter = (IDomainObjectConverter)Activator.CreateInstance(property.Attr.ConverterType);

                        sourceValue = converter.Convert(sourceValue);
                    }

                    Type type;
                    object targetObject;

                    if (property.Attr?.PropertyOf is string propertyOf && children.TryGetValue(propertyOf, out (Type propertyType, object childObject) tuple))
                    {
                        type = tuple.propertyType;
                        targetObject = tuple.childObject;
                    }
                    else
                    {
                        type = targetType;
                        targetObject = obj;
                    }

                    if (type.GetProperty(property.Attr?.Name ?? property.Name) is PropertyInfo targetProperty && targetProperty.CanWrite)
                    {
                        targetProperty.SetValue(targetObject, Convert.ChangeType(sourceValue, targetProperty.PropertyType));
                    }
                }
            }

            foreach (var child in children)
            {
                if (targetType.GetProperty(child.Key) is PropertyInfo targetProperty && targetProperty.CanWrite)
                {
                    targetProperty.SetValue(obj, child.Value.Item2);
                }
            }

            return obj;
        }

        protected void FromDomainObject<T>(T source, bool strict = false) where T : class
        {
            var thisType = GetType();

            if (source == null)
            {
                Debug.WriteLine($"Input parameter '{nameof(source)}' for method [{thisType}.{nameof(FromDomainObject)}] is null.");

                return;
            }

            var sourceType = typeof(T);

            var targetProperties = thisType.GetProperties()
                .Where(property => property.CanWrite)
                .Select(property => new { property.Name, Property = property, Attr = property.GetCustomAttribute<DomainObjectPropertyAttribute>() })
                .Where(property => property.Attr?.Skip != Conversion.All && property.Attr?.Skip != Conversion.FromDomain && ((property.Attr?.ObjectType == null && !strict) || property.Attr?.ObjectType == sourceType));

            var children = targetProperties
                .Where(property => !string.IsNullOrWhiteSpace(property.Attr?.PropertyOf))
                .Select(property => property.Attr.PropertyOf)
                .Distinct()
                .ToDictionary(propertyName => propertyName, propertyName =>
                {
                    return sourceType.GetProperty(propertyName) is PropertyInfo propertyInfo ? (propertyInfo.PropertyType, propertyInfo.GetValue(source)) :
                        throw new ArgumentException($"Failed to infer Type of Property '{propertyName}' for '{sourceType.GetType()}'");
                });

            foreach (var property in targetProperties)
            {
                Type type;
                object obj;

                if (property.Attr?.PropertyOf is string propertyOf)
                {
                    type = children[propertyOf].Item1;
                    obj = children[propertyOf].Item2;
                }
                else
                {
                    type = sourceType;
                    obj = source;
                }

                if (type.GetProperty(property.Attr?.Name ?? property.Name)?.GetValue(obj) is object sourceValue)
                {
                    property.Property.SetValue(this, Convert.ChangeType(sourceValue, property.Property.PropertyType));
                }
            }
        }

        protected IMvxAsyncCommand CreateCommand(ref IMvxAsyncCommand command, Func<Task> execute, Func<bool> canExecute = null, bool allowConcurrentExecutions = false)
        {
            return command ?? (command = new MvxAsyncCommand(execute, canExecute, allowConcurrentExecutions));
        }

        protected IMvxAsyncCommand<T> CreateCommand<T>(ref IMvxAsyncCommand<T> command, Func<T, Task> execute, Func<T, bool> canExecute = null, bool allowConcurrentExecutions = false)
        {
            return command ?? (command = new MvxAsyncCommand<T>(execute, canExecute, allowConcurrentExecutions));
        }
    }

    public abstract class ViewModelBase<TParameter> : ViewModelBase, IMvxViewModel<TParameter>
        where TParameter : class, new()
    {
        public ViewModelBase(IMvxNavigationService navigationService) : base(navigationService) { }

        public virtual void Prepare(TParameter parameter)
        {
            base.Prepare();

            FromDomainObject(parameter);
        }
    }

    public abstract class ViewModelBase<TParameter, TResult> : ViewModelBase<TParameter>, IMvxViewModel<TParameter, TResult>
        where TParameter : class, new()
        where TResult : class, new()
    {
        public ViewModelBase(IMvxNavigationService navigationService) : base(navigationService) { }

        protected virtual TResult ToDomainObject(TResult parameter = null)
        {
            return base.ToDomainObject(parameter);
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing)
            {
                CloseCompletionSource?.TrySetCanceled();
            }

            base.ViewDestroy(viewFinishing);
        }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }
    }

    public abstract class ViewModelResultBase<TResult> : ViewModelBase, IMvxViewModelResult<TResult>
        where TResult : class, new()
    {
        protected ViewModelResultBase(IMvxNavigationService navigationService)
            : base(navigationService) { }

        protected virtual TResult ToDomainObject(TResult parameter = null)
        {
            return base.ToDomainObject(parameter);
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing)
            {
                CloseCompletionSource?.TrySetCanceled();
            }

            base.ViewDestroy(viewFinishing);
        }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }
    }
}

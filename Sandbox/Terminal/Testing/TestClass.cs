using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jay.Randomization;
using JetBrains.Annotations;

namespace Jay.Sandbox
{
    public class TestClass : INotifyPropertyChanged, ITestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public TestClass()
        {
            this.Id = Randomizer.Instance.Int();
            this.Name = Randomizer.Instance.Guid().ToString();
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Mutate()
        {
            this.Id = Randomizer.Instance.Int();
            this.Name = Randomizer.Instance.Guid().ToString();
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"#{Id} - {Name}";
        }
    }
}
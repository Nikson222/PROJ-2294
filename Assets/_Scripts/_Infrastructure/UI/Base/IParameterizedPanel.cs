namespace _Scripts._Infrastructure.UI.Base
{
    public interface IParameterizedPanel<T> : IPanel
    {
        void SetParameters(T parameters);
    }
}


namespace Surreal.Mathematics {
  public interface ICanSlice<out T> {
    public T Slice(int offsetX, int offsetY, int width, int height);
  }
}
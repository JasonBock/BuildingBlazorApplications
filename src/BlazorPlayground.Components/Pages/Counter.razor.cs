namespace BlazorPlayground.Components.Pages;

public partial class Counter
{
	private int currentCount;

   private void IncrementCount() => 
		this.currentCount++;
}
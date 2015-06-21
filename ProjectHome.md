# Purpose #
Make backgrounding tasks under Windows.Forms simple and easy.

# Example #
Usage example:

```
using Backgrounder;

public class Form1 : Form {
	protected ProgressBar progressBar1;
	protected Button button1;

	private BackgroundHelper helper = new BackgroundHelper();

	private void button1_Click(object sender, EventArgs e) {
		// Execute code in the background.
		helper.Background(() => {
			for (int i = 0; i <= 100; i++) {
				// Continually report progress to user.
				helper.Foreground<int>(i, j => {
					progressBar1.Value = j;
				});
				// Simulate doing I/O or whatever.
				Thread.Sleep(25);
			}
		});
	}
}
```

## Example details ##
When a button is clicked, the progress bar will slowly tick up.  Meanwhile, the GUI will continue to respond normally - `Thread.Sleep()` is only done on the background thread, because the code block it is in was backgrounded with `helper.Background()`.

The `helper.Foreground()` call takes a couple more parameters, because it makes a copy of a variable before sending code to the foreground.  A copy is made so that the background thread can continue changing the variable without affecting the just-scheduled code.

# Window Messages #
The background thread listens for window messages.  A `NativeWindow` created on the background thread can intercept incoming messages for it's `Handle` in it's override of `WndProc()`.  Window messages can be sent from the background thread using standard `PostMessage()` et al.

# Timers #
If you can live with polling rather than pushing updates to the GUI, using a `System.Windows.Forms.Timer` (**not** `System.Threading.Timer`) to peform GUI updates is also a good solution.  It can be used as an alternative to `backgrounder`, or as a complementary ingredient.

With a System.Windows.Forms.Timer, `WM_TIMER` messages are injected by the operating system into the foreground / UI thread at regular intervals, at which point an event handler can take over.  The event handler can check shared variables set by code running in the background and update the GUI.

An advantage of using Windows.Forms timers is that it's very easy to control the rate of GUI updates and thus performance.  A disadvantage of using Windows.Forms timers is that you have to manually marshal data from background to foreground, and this can get somewhat tongue-in-cheek if you're using complex data structures.
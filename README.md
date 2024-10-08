# UnityTest
A testing framework for the Unity editor that helps you maintain and debug your code without requiring "assembly definition" (`*.asmdef`) files, unlike the [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html) (UTF). However, `UnityTest` also still works with `*.asmdef` files.

## Features
1. No `*.asmdef` files required.
2. Run and manage tests in an editor window.
3. Write test methods in any class. AND/OR,
4. Write test suites separate from your code.
5. Create complex testing environment prefabs.

----------------------------

## Installation
### Unity Package
1. Copy the GitHub repo link: `https://github.com/hatfullr/UnityTest.git`.
2. Go to `Window > Package Manager`
3. Click the "+" button in the top left and select "Add package from git URL..." in the dropdown.
4. Paste the GitHub repo link into the bar.
5. Click "Add".
6. Open the test manager at `Window > UnityTest Manager`.

### Simple
1. Download the `*.cs` scripts from the `Runtime` folder and place them anywhere inside your project.
2. Download `Editor/TestManager.cs` and move it into a folder named `Editor` in the `Assets` directory of your project.
3. Reload the domain.
4. Open the test manager at `Window > UnityTest Manager`.

## Quick Start
Unit testing is intended to make sure code runs properly even after other systems have been introduced. However, the following example keeps things simple. Suppose you have some Unity class called `Example`:
```C#
using UnityEngine;

public class Example : MonoBehaviour
{
    public int number;

    private void Method()
    {
        number++;
    }
}
```
Are we sure that `Method` actually increments `number` as we expect? Let's make a test to be sure:
```C#
using UnityEngine;
using UnityTest;

public class Example : MonoBehaviour
{
    public int number;

    private void Method()
    {
        number++;
    }

    [Test] // Create a unit test with the name "TestMethod"
    private void TestMethod(GameObject gameObject)
    {
        Example script = gameObject.GetComponent<Example>(); // Get the Example component
        int previous = script.number; // Store the previous value of "number"
        script.Method(); // Increment "number"
        Assert.AreEqual(previous + 1, script.number); // Check if the increment worked
    }
}
```
Open the test manager at `Window > UnityTest Manager`, check the box where you see "Method" and press the play button in the top left of the window (not the main editor play button). The editor will then enter Play mode and run the test by instantiating a `GameObject` with an attached `Example` component, and then calling `TestMethod` on it.

## Debugging
When a test fails, it will throw an exception that is visible in the console log. Unfortunately, double-clicking the message will not deliver you to the line of code where the test failed. To get there, follow these steps:
1. In the console, click the "triple dot" button in the top-right.
2. Turn on "Strip logging callstack". This removes any clutter from the stack trace in the console that might have come from `UnityTest`.
3. Click the message in the console.
4. Find the link to your script in the stack trace and click it.

Unfortunately, this is the best solution currently possible in Unity, at least until Unity adds an API for the console window.
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Image = OpenTK.Windowing.Common.Input.Image;

namespace Surreal.Internal;

internal sealed class OpenTKWindow : IDesktopWindow
{
	private readonly GameWindow window;

	public OpenTKWindow(DesktopConfiguration configuration)
	{
		var gameWindowSettings = new GameWindowSettings
		{
			RenderFrequency = 0,
			UpdateFrequency = 0,
			IsMultiThreaded = false
		};

		var nativeWindowSettings = new NativeWindowSettings
		{
			Title = configuration.Title,
			Size = new(configuration.Width, configuration.Height),
			WindowBorder = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed
		};

		window = new GameWindow(gameWindowSettings, nativeWindowSettings)
		{
			VSync = configuration.IsVsyncEnabled ? VSyncMode.On : VSyncMode.Off
		};

		if (configuration.Icon is { Format: TextureFormat.RGBA8888 })
		{
			var icon = configuration.Icon;
			var pixels = MemoryMarshal.Cast<Color, byte>(icon.Pixels);

			window.Icon = new WindowIcon(new Image(icon.Width, icon.Height, pixels.ToArray()));
		}

		// TODO: set default width/height based on monitor resolution
		// TODO: center on-screen

		window.Resize += _ => Resized?.Invoke(Width, Height);

		window.MakeCurrent();
	}

	public event Action<int, int>? Resized;

	public bool IsFocused => window.IsFocused;
	public bool IsClosing => window.IsExiting;

	public int Width
	{
		get => window.Size.X;
		set
		{
			Debug.Assert(value > 0, "value > 0");
			window.Size = new(value, window.Size.Y);
		}
	}

	public int Height
	{
		get => window.Size.Y;
		set
		{
			Debug.Assert(value > 0, "value > 0");
			window.Size = new(window.Size.X, value);
		}
	}

	public bool IsVisible
	{
		get => window.IsVisible;
		set => window.IsVisible = value;
	}

	public bool IsCursorVisible
	{
		get => window.CursorVisible;
		set => window.CursorVisible = value;
	}

	public string Title
	{
		get => window.Title;
		set => window.Title = value;
	}

	public bool IsVsyncEnabled
	{
		get => window.VSync != VSyncMode.Off;
		set => window.VSync = value ? VSyncMode.On : VSyncMode.Off;
	}

	public KeyboardState GetKeyboardState()
	{
		return window.KeyboardState;
	}

	public MouseState GetMouseState()
	{
		return window.MouseState;
	}

	public void Update()
	{
		if (!IsClosing)
		{
			window.MakeCurrent();
			window.ProcessEvents();
		}
	}

	public void Present()
	{
		if (!IsClosing)
		{
			window.SwapBuffers();
		}
	}

	public void Dispose()
	{
		window.Dispose();
	}
}

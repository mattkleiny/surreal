using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using OpenTK.Graphics;
using OpenTK.Platform;
using Surreal.Platform;

namespace Surreal.Editor.Internal {
  internal sealed class EditorWindowHost : HwndHost, IDesktopWindow, IWindowInfo {
    private GraphicsContext context;

    public event Action<int, int> Resized;

    public string Title {
      get => Name;
      set { } // no-op
    }

    public new int Width {
      get => (int) base.Width;
      set => base.Width = value;
    }

    public new int Height {
      get => (int) base.Height;
      set => base.Height = value;
    }

    public new bool IsVisible {
      get => base.IsVisible;
      set { } // no-op
    }

    public bool IsVsyncEnabled {
      get => true;
      set { } // no-op
    }

    public bool IsCursorVisible {
      get => true;
      set { } // no-op
    }

    public bool IsClosing => false;

    public void Update() {
      if (!IsClosing) {
        context.MakeCurrent(this);
      }
    }

    public void Present() {
      context.SwapBuffers();
    }

    protected override HandleRef BuildWindowCore(HandleRef parent) {
      context = new GraphicsContext(GraphicsMode.Default, this, 3, 0, GraphicsContextFlags.Debug);

      context.MakeCurrent(this);
      context.LoadAll();

      return parent;
    }

    protected override void DestroyWindowCore(HandleRef hwnd) {
      context.Dispose();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
      base.OnRenderSizeChanged(sizeInfo);

      var width  = (int) sizeInfo.NewSize.Width;
      var height = (int) sizeInfo.NewSize.Height;

      Resized?.Invoke(width, height);
    }
  }
}
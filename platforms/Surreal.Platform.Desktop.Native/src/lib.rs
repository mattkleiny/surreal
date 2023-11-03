//! A set of native extensions for Surreal.

use std::ffi::CStr;

use winit::{
  event::{Event, WindowEvent},
  event_loop::EventLoop,
  window::WindowBuilder,
};

/// The configuration for the host.
#[repr(C)]
pub struct HostConfiguration {
  window_title: *const i8,
  window_width: u32,
  window_height: u32,
  is_resizable: bool,
}

/// Starts the main host loop.
#[no_mangle]
pub unsafe extern "C" fn start_host(configuration: HostConfiguration) {
  let event_loop = EventLoop::new().unwrap();
  let window = WindowBuilder::new()
    .with_title(
      CStr::from_ptr(configuration.window_title)
        .to_str()
        .expect("Failed to convert window title to string"),
    )
    .with_inner_size(winit::dpi::LogicalSize::new(
      configuration.window_width,
      configuration.window_height,
    ))
    .with_resizable(configuration.is_resizable)
    .build(&event_loop)
    .unwrap();

  event_loop
    .run(move |event, elwt| match event {
      Event::WindowEvent {
        event: WindowEvent::CloseRequested,
        ..
      } => {
        elwt.exit();
      }
      Event::AboutToWait => {
        window.request_redraw();
      }
      Event::WindowEvent {
        event: WindowEvent::RedrawRequested,
        ..
      } => {
        // TODO: redraw
      }
      _ => (),
    })
    .expect("Failed to run event loop");
}

#[cfg(test)]
mod test {
  #[test]
  fn it_works() {
    assert_eq!(2 + 2, 4);
  }
}

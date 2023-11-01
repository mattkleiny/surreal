//! A set of native extensions for Surreal.

#[no_mangle]
pub unsafe extern "C" fn say_hello(name: *const u8, length: usize) {
    let message = String::from_raw_parts(name as *mut u8, length, length);

    println!("Hello, {}!", message);
}

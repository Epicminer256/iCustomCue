// use std::process::Command;
// for when using cli commands and stuffs
pub mod corshair{
    pub struct Corsair {
        pub path: String, 
    } 
    impl Corsair {
        pub fn new(passed_string: &str) -> Self {
            Corsair {
                path: passed_string.to_string()
            }
        }

        pub fn get_path(&self){
            println!("{}", self.path)
        }
    }
}

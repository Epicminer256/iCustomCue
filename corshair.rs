// use std::process::Command;
use std::path::Path;

pub fn is_icue_path(path: &str) -> bool{
    return Path::new(&path).is_dir();
}

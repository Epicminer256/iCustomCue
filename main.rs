mod corshair;

fn main() {
    let default_path = "C:\\Program Files\\Corsair\\Corsair iCUE5 Software\\iCUE.exe";

    let does_cue_exist = corshair::is_icue_path(default_path);
    println!("{}", does_cue_exist);
}

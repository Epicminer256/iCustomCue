mod corshair;

fn main() {
    let instance = corshair::corshair::Corsair{
        path: "ur papa's place".to_string(),
    };
    println!("{}", instance.path)
    
}

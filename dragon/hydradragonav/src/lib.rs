#![cfg(windows)]

pub mod bloom_filter;
pub mod boot_scanner;
pub mod disinfector;
pub mod hash_scanner;
pub mod memory_scanner;
pub mod metrics;
pub mod ml;
pub mod pipeline;
pub mod quarantine;
pub mod file_pum_scanner;
pub mod fix_registry;
pub mod registry_scanner;
pub mod remediation;
pub mod settings;
pub mod startup_scanner;
pub mod takeown;
pub mod restart_disinfect;
pub mod scanner;
pub mod types;
pub mod verdict;



pub use scanner::Scanner;
pub use types::{Error, ScanResult as ClamavScanResult, CL_CLEAN, CL_VIRUS};
pub use verdict::{EngineResult, ScanResult, Verdict};

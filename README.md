# SimvarWatcher-MFWASM

This is an extension of the well-known SimConnect sample application.

The issue with the standard version is that it gives access only to the regular variables as this is the only thing provided by the SimConnect interface.

Fortunately, the MobiFlight project implemented a [stand-alone WASM module](https://github.com/Mobiflight/MobiFlight-WASM-Module) which gives access to all internal variables.

To make it work - install the WASM module using the [MobiFlight instructions](https://github.com/MobiFlight/MobiFlight-Connector/tree/main/MSFS2020-module).

Our version of SimvarWatcher assumes all variables whose definition starts from the opening bracket symbol "(" should be passed to the WASM module. The expected format is "(L:MyVar)".

The WASM module variables can be only of the decimal type. You can set them if they are writeable.

All variable changes are performed by generating code line the following "5 (>L:MyVar)" and executing with [execute_calculator_code](https://docs.flightsimulator.com/html/Programming_Tools/WASM/Gauge_API/execute_calculator_code.htm).

As a back-side effect - you can execute ANY sim command [execute_calculator_code](https://docs.flightsimulator.com/html/Programming_Tools/WASM/Gauge_API/execute_calculator_code.htm) - just add an opening bracket "(" the command string, place it into the Value field and press the "Try set value >" button.

This project is also a good help to those who would like to integrate the MobiFlight WASM module into their code. Check the [comparison of the initial version and current](https://github.com/rmaryan/SimvarWatcher-MFWASM/compare/original_simvarwatcher...master) for an easy to easy-to-follow view.

Those who don't like to build the app on their own can download a ready to use executable here: [release_binary.zip](Release/release_binary.zip).

![Main Window](main_window.png)

# Liked some of my work?

<a href="https://www.buymeacoffee.com/maryanR" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>


# Overview

<img src="./SmithChartTool/Images/Logo/sct_hr.png"/>

SmithChartTool is used to observe transformational effects of different microwave elements (such as lumped elements, stubs, transmission lines, etc.) on impedance elements (from load to source).
<br>

## Trivia

Since a proprietary Smith-Chart tool included in some high-end software-package literally saved me during Master's thesis I searched for standalone (open-source) Smith-Chart tools for not being reliant upon licenses. I found out, that several tools on the internet are either not "pretty", yet complicated to use, or just not modern enough. 
<br>
In this project, I tried my best using MVVM (Model-View-ViewModel) approach in combination with CustomControls, Bindings, Validaters, Converters, Drag&Drop functionality and other WPF magic based on modern C# approaches (async, lambda, out parameters, auto properties). Impedance / admittance Smith-Charts are generated using the fantastic [oxyplot](https://github.com/oxyplot/oxyplot) library.

## Current features (some...)

- Input parameters may be input using SI-prefixes
- Drag&Drop of elements into schematic view (including rearrangement).
- Transformational curves and intermediate input impedances are shown.
- Projects can be stored and opened from disc.
- Smith-Chart itself may be exported to an image to use it in your documentation.

## Releases

[github-release-link]: https://github.com/bonbytes/SmithChartTool/releases/

## Build status


## Documentation

As starting point, enter target frequency and reference impedance, drag & change elements and you directly may observe the resulting Smith-Chart transformation to match your circuit as you like.

[github-page]: https://bonbytes.github.io/SmithChartTool-/

## Contributing

This project welcomes contributions of all types. Help spec'ing, design, documentation, finding bugs are ways everyone can help on top of coding features / bug fixes. I am excited to work with the community to build a tool for every (RF-) engineer around.

### State of code 

SmithChartTool is still a very fluidic project and I am actively working out of this repository.  Periodic re-structuring/refactoring of the code could appear to make it easier to comprehend, navigate, build, test, and contribute to, so **DO expect significant changes to code layout on a regular basis**.

### License Info

This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

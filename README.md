# InitialMargin

This library is a C# implementation of the following `Initial Margin` models:
 - [Standard Initial Margin Model 2.1 (SIMM™ 2.1)](https://www.isda.org/2018/08/27/isda-publishes-isda-simm-2-1/) developed by [ISDA](https://www.isda.org);
 - [Schedule Initial Margin Model](https://www.bis.org/bcbs/publ/d475.htm) developed by [BCBS](https://www.bis.org/bcbs/).
 
It has been built to be compatible with [Common Risk Interchange Format (CRIF™)](https://www.isda.org/a/owEDE/risk-data-standards-v1-36-public.pdf) files.

## Main Features

InitialMargin is...

 * `Easy-to-Deploy`: all what it requires is compliant input data, either in the form of a `CRIF` file or as a runtime-generated list of data entities;
 * `Easy-to-Maintain`: periodic updates to Initial Margin models are easy to integrate and require only minor changes to the library algorithms;
 * `Stable`: the library implements strong data validation routines and a very detailed exception handling framework.

## Requirements
 
The library is os-agnostic (it has been developed under .NET Standard 2.0) and platform-agnostic, (both x86 and x64 environments are supported). The project targets Visual Studio 2017.

# Playing with Mountebank

This is a small .NET Core xUnit application to try out `Mountebank`. An open source tool to provide cross-platform, multi-protocol test doubles over the wire.

![](http://www.mbtest.org/images/overview.gif)

There is a `pipeline.yml` file in this repository, which contains a build stage to install and run Mountebank. Run the tests to initiate HTTP call over the imposter, defined in the test.

#### Resources

- [Mountebank](http://www.mbtest.org) *(Official page)*
- [MbDotNet](https://github.com/mattherman/MbDotNet) *(GitHub)*
- [Stubbing REST services with Mountebank](https://simonvane.com/2018/04/27/testing-rest-with-mbdotnet) *(Simon Vane)*
# Drive Libraries

Package of libraries for working with Euro Truck Simulator 2.


## Compatibility and usage with American Truck Simulator

Although the libraries should work (with minor modifications) with ATS, I don't own the game and can't test with it. If you feel like contributing, I'd be happy to review and eventually merge pull requests.


### Documentation

The source code should all be documented. If not, feel free to file a bug on the [GitLab repository](https://gitlab.com/drive-ets/drive-libs).


## Tests

Drive libraries are tested but given their open source nature, sharing tests is not that simple given some required test data is property of SCS Software and I can't share it.


### SCSArchive tests

In order to run these tests, you'll have to update your Drive.ScsArchive.Tests.ExternalPaths.GameInstallPath property to point to your ETS2 installation folder. This is needed to test access to the \*.scs archives - only read access is required, of course.


### SiiFile tests

These just won't run given they require some save data from the game and I'm not sure I can legally share that. If and when I get confirmation from SCS Software, those will be pushed to this repo as well. They are still included as example code.


## License

Drive is licensed under the MIT license. See the [LICENSE](./LICENSE.txt) file for further information.


## Bugs

If you find any bugs, file an issue at the [GitLab repository](https://gitlab.com/drive-ets/drive-libs).
*Please remember the issue list is not a "how do I..." bulletin board.*

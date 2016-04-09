# Bookproto

Generate protobuf parse code with custom template.

# Feture

* Works on Windows,Linux and Mac OX S
* Support `template` for any language
* Default implements `C#, PHP`
* Batch works
* Filter by: `file name`

# How to use

1. Install php and make sure php in `PATH`
2. Edit `bookproto.php`, add base params, for example:

		$params['inputPath'] = 'protoDir';
		$params['outputPath'] = 'outputDir';
		$params['codeType'] = 'C#';
		parseProtos($params);

3. Run command:

		php bookproto.php

	For Windows, you can also put php in `runtime` dir and run `bookproto.bat`.

# Requirement

* php 5.3 or higher

# License

Bookproto is licensed under the MIT License. See the [LICENSE](https://opensource.org/licenses/MIT) file for more information.

<?php
require 'bookproto/Bookproto.php';

$params = array(
    //frequently used
    'inputPath' => "", //directory or name*.ext* file
    'outputPath' => "protos", //directory
    'codeType' => "C#",

    //barely used
    'prefix' => "",
    'parentClass' => "",
    'templateDir' => "", //the codeType's template directory
    'commentSymbol' => "#", //like #xxx.proto file will be ignored
    'onlySimpleName' => true, //file name contains letter, number or underline only
    'genPackageDir' => true, //auto generate package directory
);


$params['inputPath'] = 'test/*.proto';
$params['outputPath'] = 'protos';
$params['codeType'] = 'C#';
parseProtos($params);


// 
// add more workers
// 
// $params['inputPath'] = 'test/*.proto';
// $params['outputPath'] = 'protos';
// $params['codeType'] = 'C#';
// parseProtos($params);


<?php
/**
 * Copyright (c) 2016, bookrpg, All rights reserved.
 * @author llj <wwwllj1985@163.com>
 * @license The MIT License
 */

define('APP_ROOT', __DIR__ . '/');

if (!class_exists('ProtobufMessage')) {
    require_once APP_ROOT . 'vendor/PHP-Protobuf/ProtobufMessage.php';
}

require_once APP_ROOT . 'vendor/PHP-Protobuf/ProtobufCompiler/ProtobufParser.php';
require_once APP_ROOT . 'TplEngine.php';
require_once APP_ROOT . 'Util.php';
require_once APP_ROOT . 'PinYin.php';

const TAB = "    ";
const EOL = "\n";

$file = '';

$codeGenerator = array('C#' => APP_ROOT . 'template/C#');

$theParams = null;

$defaultParams = array(
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

function addCodeGenerator($codeType, $templateDir)
{
    $GLOBALS['codeGenerator'][$codeType] = $templateDir;
}

function parseProtos($params)
{
    Util::restWarningError();
    $protoCount = 0;
    global $defaultParams, $codeGenerator;

    if ($params == null || !is_array($params)) {
        $params = $defaultParams;
    } else {
        foreach ($defaultParams as $k => $v) {
            if (!isset($params[$k])) {
                $params[$k] = $v;
            }
        }
    }

    $path = Util::getAbsolutePath($params['inputPath']);
    $params['fileSavePath'] = '';

    //if outputPath is not absolete, then it is relative to inputPath,
    //not relative to pwd
    if ($params['outputPath'] != '') {
        $params['fileSavePath'] = Util::getAbsolutePath(
            $params['outputPath'],
            is_dir($path) ? $path : dirname($path)
        );
    }

    $tplDir = $params['templateDir'];
    if (!isset($codeGenerator[$params['codeType']]) && $tplDir == '') {
        Util::error('not support codeType: ' . $params['codeType']);
        echo PHP_EOL;
        return;
    } else {
        if ($tplDir == '') {
            $tplDir = $codeGenerator[$params['codeType']];
        }
        $tplDir = Util::getAbsolutePath($tplDir);
        $params['templateDir'] = $tplDir;
        Util::autoload($tplDir);
    }

    global $theParams;
    $theParams = $params;

    echo '---------------------------------' . PHP_EOL;
    echo 'start parse proto at: ' . $path . PHP_EOL;
    echo PHP_EOL;

    try {
        if (is_dir($path)) {
            $dir = Util::addDirSeparator($path);
            $needCheckExt = true;
            $files = scandir($path);
        } else {
            $dir = '';
            $needCheckExt = !preg_match('/\.[^.\/]+$/', $path);
            $files = glob($path);
        }

        if ($files === false) {
            Util::error('can not find files at: ' . $path);
            echo PHP_EOL;
            return;
        }

        foreach ($files as $filename) {
            $basename = basename($filename);
            $filename = $dir . $filename;

            if (is_file($filename) &&
                !isIgnoreFile($basename) &&
                (($needCheckExt && Util::isExtension($basename, 'proto')) ||
                    !$needCheckExt)
            ) {
                $protoCount++;
                echo '--------read file:' . $filename . PHP_EOL;

                try {
                    parseProto($filename);
                } catch (Exception $e) {
                    Util::error($e->getMessage());
                }
            }
        }
    } catch (Exception $e) {
        Util::error($e->getMessage());
    }

    echo PHP_EOL;
    echo 'finish parse proto at: ' . $path . PHP_EOL;
    echo 'total file count: ' . $protoCount . PHP_EOL;
    echo 'error count: ' . Util::$errorCount . PHP_EOL;
    echo 'warning count: ' . Util::$warningCount . PHP_EOL;
    echo '---------------------------------' . PHP_EOL;
}

function parseProto($filename)
{
    global $file;
    $parser = new ProtobufParser();
    $file = $parser->bkParse($filename);
    $package = $file->getPackage();
    $name = $file->getName();
    $messages = $file->getMessages();

    createEnums();

    foreach ($messages as $msg) {
        createClass($msg);
    }
}

function createEnums()
{
    global $file, $theParams;
    $params = $theParams;

    if (($tpl = getTpl('enum')) == '') {
        return;
    }

    $enums = $file->getEnums();
    if (count($enums) == 0) {
        return;
    }

    $package = $file->getPackage();
    $prefix = $params['prefix'];

    ob_start();
    include TplEngine::compileFile($tpl);
    $str = ob_get_contents();
    ob_clean();

    $filename = Util::getMainName($file->getName());
    if (!$params['onlySimpleName']) {
        $filename = convertFileName($filename);
    }
    $filename = ucfirst($filename) . 'Enums';

    $fileSavePath = getFileSavePath($filename, $tpl);
    Util::saveToFile($fileSavePath, $str);
}

function createClass($msg)
{
    global $file, $theParams;
    $params = $theParams;

    if (($tpl = getTpl('class')) == '') {
        return;
    }

    $prefix = $params['prefix'];
    $parentClass = $params['parentClass'];
    $package = $file->getPackage();
    $name = $msg->getName();
    $enums = $msg->getEnums();
    $nestedMessages = $msg->getNested();
    $fields = $msg->getFields();

    ob_start();
    include TplEngine::compileFile($tpl);
    $str = ob_get_contents();
    ob_clean();

    $fileSavePath = getFileSavePath($name, $tpl);
    Util::saveToFile($fileSavePath, $str);
}

function insertNestedClass($msg)
{
    global $file, $theParams;
    $params = $theParams;

    if (($tpl = getTpl('nestedClass')) == '') {
        return;
    }

    $prefix = $params['prefix'];
    $parentClass = $params['parentClass'];
    $package = $file->getPackage();
    $name = $msg->getName();
    $enums = $msg->getEnums();
    $nestedMessages = $msg->getNested();
    $fields = $msg->getFields();

    include TplEngine::compileFile($tpl);
}

function insertNestedEnum($enum)
{
    global $file, $theParams;
    $params = $theParams;

    if (($tpl = getTpl('nestedEnum')) == '') {
        return;
    }

    $prefix = $params['prefix'];
    include TplEngine::compileFile($tpl);
}

function tab($num = 1)
{
    $str = '';
    while ($num-- > 0) {
        $str .= TAB;
    }
    return $str;
}

function isMessage($field)
{
    return $field->getTypeDescriptor() instanceof MessageDescriptor;
}

function isEnum($field)
{
    return $field->getTypeDescriptor() instanceof EnumDescriptor;
}

function getFileSavePath($name, $tpl)
{
    global $file, $theParams;

    $fileSavePath = $theParams['fileSavePath'] == '' ?
    dirname($file->getName()) : $theParams['fileSavePath'];

    if ($theParams['genPackageDir']) {
        $package = $file->getPackage();
        $fileSavePath = Util::addDirSeparator($fileSavePath) .
        str_replace('.', '/', $package);
    }

    return Util::addDirSeparator($fileSavePath) .
    $theParams['prefix'] . $name . '.' . Util::getExtension($tpl);
}

function isIgnoreFile($filename)
{
    $params = $GLOBALS['theParams'];
    //中文.proto 非字母、数字、下划线命名的文件
    if ($params['onlySimpleName'] && !preg_match('/^[a-zA-Z]\w*\.?\w*$/', $filename)) {
        return true;
    }

    //#xxx.proto 文件被注释
    $commentSymbol = $params['commentSymbol'];
    if ($commentSymbol != '' && strpos($filename, $commentSymbol) === 0) {
        return true;
    }
}

function getTpl($tplName)
{
    $params = $GLOBALS['theParams'];
    $tplDir = $params['templateDir'];
    $arr = glob(Util::addDirSeparator($tplDir) . $tplName . '.*');
    if ($arr === false || count($arr) == 0) {
        Util::error("can not find template: $tplName in $tplDir");
        return '';
    }
    return $arr[0];
}

function convertFileName($filename)
{
    $py = new PinYin();
    return $py->getFirstPY($filename);
}

function insertTpl($filename)
{
    if (($tpl = getTpl($filename)) == '') {
        return '';
    }

    return TplEngine::compileFile($tpl);
}

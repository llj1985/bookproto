<?php
/**
 * Copyright (c) 2016, bookrpg, All rights reserved.
 * @author llj <wwwllj1985@163.com>
 * @license The MIT License
 */

class Util
{
    public static function autoload($dir)
    {
        foreach (glob($dir . '/*.php') as $file) {
            include_once $file;
        }
    }

    public static function removeComment($str, $commentSymbol)
    {
        if ($str == '') {
            return $str;
        }

        if (strpos($str, $commentSymbol) === 0) {
            $str = substr($str, strlen($commentSymbol));
        }
        return $str;
    }

    public static function isExclude($filename, array $excludes)
    {
        foreach ($excludes as $pattern) {
            if (fnmatch(self::getAbsolutePath($pattern), $filename)) {
                return true;
            }
        }
        return false;
    }

    /**
     * return directory
     * xxx/name.txt => xxx
     * xxx/yyy/ => xxx/yyy
     */
    public static function getDir($filename)
    {
        if (is_dir($filename)) {
            return rtrim($filename, "/\\");
        } else if (is_file($filename)) {
            return dirname($filename);
        }

        $filename = preg_replace("/\w*[*?.]\w*$/", '', $filename);
        return rtrim($filename, "/\\");
    }

    /**
     * return path without extentsion
     * xxx/name.txt => xxx/name
     * xxx/yyy/ => xxx/yyy
     */
    public static function getBasePath($filename)
    {
        if (is_dir($filename)) {
            return rtrim($filename, "/\\");
        }
        return preg_replace("/\.([^\\\.\/]+)$/", '', $filename);
    }

    /**
     * xxx/name.txt => name
     * xxx/yyy/ => yyy
     */
    public static function getMainName($filename)
    {
        if ($filename == '') {
            return $filename;
        }

        $lastChar = $filename[strlen($filename) - 1];
        if ($lastChar == '/' || $lastChar == '\\') {
            if (preg_match("/([^\/\\\]+)[\/\\\]+$/", $filename, $matches)) {
                return $matches[1];
            }
        }

        $filename = preg_replace("/\.([^\\\.\/]+)$/", '', $filename);
        if (preg_match("/[^\/\\\]+$/", $filename, $matches)) {
            return $matches[0];
        }

        return '';
    }

    public static function getExtension($filename)
    {
        if (preg_match("/\.([^\\\.\/]+)$/", $filename, $matches)) {
            return $matches[1];
        }
        return '';
    }

    /**
     * check file extention
     * @param  string  $filename
     * @param  string or array  $ext  'txt' or array('txt','md')
     * @return boolean
     */
    public static function isExtension($filename, $ext)
    {
        if ($filename == '' || $ext == '') {
            return false;
        }

        if (!is_array($ext)) {
            $ext = array($ext);
        }

        foreach ($ext as $v) {
            if ($v[0] != '.') {
                $v = '.' . $v;
            }
            if (strripos($filename, $v) == strlen($filename) - strlen($v)) {
                return true;
            }
        }

        return false;
    }

    public static function unifyDirSeparator($path)
    {
        if ($path == '') {
            return $path;
        }
        return str_replace("\\", "/", $path);
    }

    public static function addDirSeparator($path)
    {
        if ($path == '') {
            return $path;
        }
        
        return rtrim($path,"/\\") . "/";
    }

    public static function getAbsolutePath($path, $relativePath = null)
    {
        if (strpos($path, '/') !== 0 && !preg_match('/^[a-zA-Z]:/', $path)) {
            //当前pwd或本文件的父目录作为相对目录的起点
            if ($relativePath == null && ($relativePath = getcwd()) === false) {
                $relativePath = dirname(__DIR__);
            }

            $path = $relativePath . '/' . $path;
        }

        return self::removeDotSlash(str_replace("\\", "/", $path));
    }

    /**
     * a/b/c/../.././/d => a/d
     */
    public static function removeDotSlash($path)
    {
        $path = str_replace('//', '/', $path);

        if (strpos($path, './') === false) {
            return $path;
        }
        $arr = explode('/', $path);
        for ($i = 0; $i < count($arr); $i++) {
            if ($arr[$i] == '..') {
                if ($i > 1) {
                    array_splice($arr, $i - 1, 2);
                    $i -= 2;
                } else {
                    array_splice($arr, $i, 1);
                    $i--;
                }
            } else if ($arr[$i] == '.' || $arr[$i] == '') {
                array_splice($arr, $i, 1);
                $i--;
            }
        }

        return implode($arr, '/');
    }

    public static function saveToFile($filename, $content)
    {
        if (!file_exists($filename)) {
            $dir = dirname($filename);
            if (!file_exists($dir)) {
                @mkdir($dir, 0777, true);
            }
        } else {
            @unlink($filename);
        }

        return file_put_contents($filename, $content);
    }

    public static $warningCount = 0;
    public static $errorCount = 0;

    public static function restWarningError()
    {
        self::$warningCount = 0;
        self::$errorCount = 0;
    }

    public static function warning($msg)
    {
        self::$warningCount++;
        echo 'warning: ' . $msg . PHP_EOL;
    }

    public static function error($msg)
    {
        self::$errorCount++;
        echo 'error: ' . $msg . PHP_EOL;
    }
}

<?php
/**
 * Copyright (c) 2016, bookrpg, All rights reserved.
 * @author llj <wwwllj1985@163.com>
 * @license The MIT License
 */


class TplCache
{
    public $filename;
    public $cTime;
}

class TplEngine
{

    private static $caches = array();

    public static function compileFile($filename)
    {
        if (!is_file($filename)) {
            return '';
        }

        $compiledFilename = __DIR__ . '/cache/' . basename($filename) . '.php';

        $cache = @self::$caches[$filename];

        if (!$cache || 
            $cache->cTime === false || 
            $cache->cTime != filectime($filename) || 
            !file_exists($compiledFilename)) {

            $str = self::compile(file_get_contents($filename));
            Util::saveToFile($compiledFilename, $str);

            $cache = new TplCache();
            $cache->filename = $filename;
            $cache->cTime = filectime($filename);
            self::$caches[$filename] = $cache;
        }

        return $compiledFilename;
    }

    public static function compile($str)
    {
        $str = str_replace('{%:', '{%echo ', $str);

        $str = preg_replace(
            '/(\{%\s*)([A-Z_]+|\$\w+|\$\w+\[.+\])(\s*%\})/',
            '\1echo \2;\3',
            $str
        );

        $str = preg_replace(
            '/(\{%\s*)(\$\w+\s*->\s*\w+|\$\w+\s*->\s*\w+\[.+\])(\s*%\})/',
            '\1echo \2;\3',
            $str
        );

        //替换非echo行首空白:全角空格、空格、水平制表符
        $str = preg_replace(
            '/([\r\n])([\x{3000}\x20\t]+)(\{%)(?!\s*echo )/u',
            '\1\3',
            $str
        );

        $str = str_replace('{%', '<?php ', $str);
        $str = str_replace('%}', ' ?>', $str);

        return $str;
    }
}

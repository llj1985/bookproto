<?php
/**
 * Copyright (c) 2016, bookrpg, All rights reserved.
 * @author llj <wwwllj1985@163.com>
 * @license The MIT License
 */

/**
 * you can modify this file for your template
 */

$GLOBALS['typeMap'] = array(
    'double' => 1,
    'float' => 4,
    'int32' => 5,
    'int64' => 5,
    'uint32' => 5,
    'uint64' => 5,
    'sint32' => 6,
    'sint64' => 6,
    'fixed32' => 2,
    'fixed64' => 3,
    'sfixed32' => 2,
    'sfixed64' => 3,
    'bool' => 8,
    'string' => 7,
    'bytes' => 7,
);

// const PB_TYPE_DOUBLE     = 1;
//     const PB_TYPE_FIXED32   = 2;
//     const PB_TYPE_FIXED64   = 3;
//     const PB_TYPE_FLOAT      = 4;
//     const PB_TYPE_INT        = 5;
//     const PB_TYPE_SIGNED_INT = 6;
//     const PB_TYPE_STRING     = 7;
//     const PB_TYPE_BOOL       = 8;

function getMapedType($field)
{
    global $typeMap;
    $type = $field->getType();
    return isset($typeMap[$type]) ? $typeMap[$type] : $type;
}

function getFieldType($field, $prefix = '')
{
    if (isEnum($field)) {
        return 5;
    } 

    $type = getMapedType($field);

    if (isMessage($field)) {
        $type = $prefix . $type;
        $package = getPackage($field->getTypeDescriptor(), $prefix);

        if ($package != '') {
            $type = "'\\" . $package . "\\" . $type . "'";
        }
    }

    return $type;
}

function convertPackage($package)
{
    return str_replace('.', '\\', $package);
}

function getPackage(DescriptorInterface $descriptor, $prefix = '')
{
    $namespace = array();

    $containing = $descriptor->getContaining();

    while (!is_null($containing)) {
        $namespace[] = $prefix . $containing->getName();
        $containing = $containing->getContaining();
    }

    $package = $descriptor->getFile()->getPackage();

    if (!empty($package)) {
        $namespace[] = convertPackage($package);
    }

    $namespace = array_reverse($namespace);

    $name = implode('\\', $namespace);
    return $name;
}


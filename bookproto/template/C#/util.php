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
    'double' => 'double',
    'float' => 'float',
    'int32' => 'int',
    'int64' => 'long',
    'uint32' => 'uint',
    'uint64' => 'ulong',
    'sint32' => 'int',
    'sint64' => 'long',
    'fixed32' => 'uint',
    'fixed64' => 'ulong',
    'sfixed32' => 'int',
    'sfixed64' => 'long',
    'bool' => 'bool',
    'string' => 'string',
    'bytes' => 'byte[]',
);

$GLOBALS['wireTypeMap'] = array(
    'double' => 'Fixed64',
    'float' => 'Fixed32',
    'int32' => 'Varint',
    'int64' => 'Varint',
    'uint32' => 'Varint',
    'uint64' => 'Varint',
    'sint32' => 'Varint',
    'sint64' => 'Varint',
    'fixed32' => 'Fixed32',
    'fixed64' => 'Fixed64',
    'sfixed32' => 'Fixed32',
    'sfixed64' => 'Fixed64',
    'bool' => 'Varint',
    'string' => 'LengthDelimited',
    'bytes' => 'LengthDelimited',
);

function getWireType($field)
{
    global $wireTypeMap;
    $type = $field->getType();
    if (isset($wireTypeMap[$type])) {
        return $wireTypeMap[$type];
    } else if ($field->getTypeDescriptor() instanceof EnumDescriptor) {
        return 'Varint';
    } else {
        return 'LengthDelimited';
    }
}

function getMapedType($field)
{
    global $typeMap;
    $type = $field->getType();
    return isset($typeMap[$type]) ? $typeMap[$type] : $type;
}

function getFieldType($field, $prefix='')
{
    $type = getFieldNotListType($field, $prefix);

    if ($field->isRepeated()) {
        $type = "List<" . $type . ">";
    }
    return $type;
}

function getFieldNotListType($field, $prefix='')
{
    $type = getMapedType($field);

    if (!$field->isProtobufScalarType()) {
        $type = $prefix . $type;
        $package = $field->getNamespace();
        if ($package != '') {
            $type = $package . '.' . $type;
        }
    }

    return $type;
}

function getFieldReader($field)
{
    if ($field->isProtobufScalarType()) {
        return 'read_TYPE_' . strtoupper($field->getType());
    } else if ($field->getTypeDescriptor() instanceof EnumDescriptor) {
        return 'read_TYPE_ENUM';
    } else {
        return 'read_TYPE_MESSAGE';
    }
}

function getFieldWriter($field)
{
    if ($field->isProtobufScalarType()) {
        return 'write_TYPE_' . strtoupper($field->getType());
    } else if ($field->getTypeDescriptor() instanceof EnumDescriptor) {
        return 'write_TYPE_ENUM';
    } else {
        return 'write_TYPE_MESSAGE';
    }
}

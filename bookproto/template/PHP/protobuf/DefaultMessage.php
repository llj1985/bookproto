<?php
namespace bookrpg\route\impl;

use bookrpg\route\IMessage;

/**
 * headLength(uint16), not include self length
 * opcode(uint32)
 * route1(uint16)
 * route2(uint16)
 * flag(uint32)
 * ...custom head, total size: headLength - 12
 * ...body
 */
class DefaultMessage extends \ProtobufMessage implements IMessage
{
    const HEAD_LENGTH = 12;

    private $headLength;

	public $opcode = 0;

    public $route1 = 0;

    public $route2 = 0;

    public $flag = 0;

    private $client;

    public function getOpcode()
    {
    	return $this->opcode;
    }

    /**
     * @return bool is success
     */
    public function parseHead($data)
    {
        if(strlen($data) < self::HEAD_LENGTH + 2) {
            $this->opcode = 0;
            return false;
        }

        $arr = unpack('vk1/Vk2/vk3/vk4/Vk5', $data);

        $this->headLength = $arr['k1'];
        $this->opcode = $arr['k2'];
        $this->route1 = $arr['k3'];
        $this->route2 = $arr['k4'];
        $this->flag = $arr['k5'];

        return true;
    }
    
    public function serializeHead()
    {
    	return pack('vVvvV', self::HEAD_LENGTH, $this->opcode, 
            $this->route1, $this->route2, $this->flag);
    }

    public function parseBody($data)
    {
    	$this->parseFromString($data);
    }

    public function serializeBody()
    {
    	return $this->serializeToString();
    }

    public function parse($data)
    {
    	$this->parseHead($data);
        if(strlen($data) > $this->headLength + 2){
            $this->parseBody(substr($data, $this->headLength + 2));
        }
    }

    public function serialize()
    {
    	return $this->serializeHead() . $this->serializeBody();
    }

    public function reset()
    {

    }

    /**
     * @return IClient
     */
    public function getSender()
    {
        return $this->client;
    }

    public function setSender($value)
    {
        $this->client = $value;
    }
}
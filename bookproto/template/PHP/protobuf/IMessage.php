<?php
namespace bookrpg\route;

interface  IMessage
{
	public function getOpcode();

    public function parseHead($data);
    
    public function serializeHead();

    public function parseBody($data);

    public function serializeBody();

    public function parse($data);

    public function serialize();

    public function getSender();

    public function setSender($value);
}
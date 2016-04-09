	class {%echo $prefix.$name%} extends {%if($parentClass):echo ',',$parentClass;else:%}\ProtobufMessage{%endif;%} 
    {
    	/* @var array Field descriptors */
	    protected static $fields = array(
		    {%
				foreach ($fields as $field):
				$fname = $field->getName();
				$number = $field->getNumber();
				$default = $field->getDefault();
				$type = getFieldType($field);
			%}
	        {%$number%} => array(
	            'name' => '{%$fname%}',
	            {% if (isset($default)): %}
	            'default' => {%$default%},
	            {% endif; %}
	            {% if ($field->isRepeated()): %}
	            'repeated' => true,
	            {% elseif ($field->isRequired()): %}
	            'required' => true,
	            {% else: %}
	            'required' => false,
	            {% endif; %}
	            'type' => {%$type%},
	        ),
	    	{% endforeach; %}
	    );

	    /**
	     * Constructs new message container and clears its internal state
	     *
	     * @return null
	     */
	    public function __construct()
	    {
	        $this->reset();
	    }

	    /**
	     * Clears message values and sets default ones
	     *
	     * @return null
	     */
	    public function reset()
	    {
	    	{%
				foreach ($fields as $field):
				$fname = $field->getName();
				$number = $field->getNumber();
				$default = $field->getDefault();
				$value = $field->isRepeated() ? 'array()' : 'null';
				if (isset($default)) {
					$value = $default;
				}
			%}
	        $this->values[{%$number%}] = {%$value%};
	    	{% endforeach; %}
	    }

	    /**
	     * Returns field descriptors
	     *
	     * @return array
	     */
	    public function fields()
	    {
	        return self::$fields;
	    }

		{%
			foreach ($fields as $field):
			$fname = $field->getName();
			$ufname = ucfirst($fname);
			$number = $field->getNumber();
		%}
		public function get{%$ufname%}()
		{
			return $this->get({%$number%});
		}

		public function set{%$ufname%}($value)
		{
			return $this->set({%$number%}, $value);
		}
        {%if($field->isRepeated()):%}
        public function get{%$ufname%}Count()
	    {
	        return $this->count({%$number%});
	    }
        {%endif;%}
		{%endforeach;%}
    }


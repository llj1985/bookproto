    public enum {%echo $prefix.$enum->getName();%} 
    {
		{% foreach ($enum->getValues() as $field): %}
    	{%echo $field->getName()%} = {%echo $field->getValue()%},
		{% endforeach; %}
	}


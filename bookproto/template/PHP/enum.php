{%echo '<?php'%} 
/**
 * Generated by bookproto
 * Do't modify this file directly
 */

namespace {%echo convertPackage($package);%} 
{
	{% foreach ($enums as $enum): %}
    final class {%echo $prefix.$enum->getName();%} 
	{
	    {% foreach ($enum->getValues() as $field): %}
    	const {%echo $field->getName()%} = {%echo $field->getValue()%};
		{% endforeach; %}

	    /**
	     * Returns defined enum values
	     *
	     * @return int[]
	     */
	    public function getEnumValues()
	    {
	        return array(
	        	{% foreach ($enum->getValues() as $field): %}
	        	'{%echo $field->getName()%}' => self::{%echo $field->getName()%},
				{% endforeach; %}
	        );
	    }
	}

	{% endforeach; %}
}
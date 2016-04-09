	public partial class {%echo $prefix.$name%} : IMessage{%if($parentClass)echo ',',$parentClass;%} 
    {
		{%
			foreach ($fields as $field):
			$fname = $field->getName();
			$ufname = ucfirst($fname);
			$number = $field->getNumber();
			$type = getFieldType($field, $prefix);
            $default = $field->getDefault();
		%}
		private {%$type%} _{%$fname%}{%echo isset($default)?'='.$default:''%};
		public  {%$type%} {%$fname%} 
		{
			get{return _{%$fname%};}
			set{_{%$fname%} = value;_has{%$ufname%}=true;}
		}

        private bool _has{%$ufname%} = false;
        {%if($field->isRepeated()):%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%} && _{%$fname%}!=null && _{%$fname%}.Count>0;}}
        {%elseif (isMessage($field)):%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%} && _{%$fname%}!=null;}}
        {%else:%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%};}}
        {%endif;%}
        {%if($field->isRepeated() || isMessage($field)):%}
        public  void clear{%$ufname%}() {{%$fname%}=null; _has{%$ufname%}=false;}
        {%else:%}
		public  void clear{%$ufname%}() {{%echo isset($default)?$fname.'='.$default.'; ':''%}_has{%$ufname%}=false;}
        {%endif;%}

		{%endforeach;%}

		public void parseFrom(Stream stream)
		{
            {%
                foreach ($fields as $field):
                $fname = $field->getName();
                if ($field->isRequired()):
            %}
            int {%$fname%}Count = 0;
            {%
                endif; 
                endforeach;
            %}

            while (stream.Position != stream.Length)
            {
                var tag = ReadUtils.readTag(stream);
                switch (tag.number)
                {
                	case 0:
                        throw new ProtobufException("Invalid field id: 0, wrong stream");
					{%
						foreach ($fields as $field):
						$tag = $field->getNumber();
						$fname = $field->getName();
						$reader = getFieldReader($field);
						$typeName = getFieldType($field, $prefix);
						$typeName2 = getFieldNotListType($field, $prefix);

						if (isMessage($field)) {
                            $str1 = "ReadUtils.{$reader}(stream, new {$typeName2}()) as {$typeName2}";
						} else if(isEnum($field)){
                            $str1 = "({$typeName2})ReadUtils.{$reader}(stream)";
						} else {
							$str1 = "ReadUtils.{$reader}(stream)";
						}
					%}
                    case {%$tag%}:
						{%if ($field->isRepeated()):%}
						if (this.{%$fname%} == null) {
							this.{%$fname%} = new {%$typeName%}();
						}
						this.{%$fname%}.Add({%$str1%});
						{%else:%}
						this.{%$fname%} = {%$str1%};
						{%endif;%}
						{%if($field->isRequired()):%}
						{%$fname%}Count++;
						{%endif;%}
                        break;
					{%endforeach;%}
					default:
                        ReadUtils.skip(stream, tag.wireType);
                        break;
                }
            }

            {%
                foreach ($fields as $field):
                $fname = $field->getName();
                $default = $field->getDefault();
                if ($field->isRequired()):
            %}
            if ({%$fname%}Count == 0) {
                throw new ProtobufException("Required field {%$fname%} not readed");
            }
            {%
                endif; 
                endforeach;
            %}
		}

		public void writeTo(Stream stream)
		{
            {%
				foreach ($fields as $field):
				$number = $field->getNumber();
				$fname = $field->getName();
				$ufname = ucfirst($fname);
				$default = $field->getDefault();
				$writer = getFieldWriter($field);
                $wireType = getWireType($field);
				$typeName = getFieldType($field, $prefix);
				$typeName2 = getFieldNotListType($field, $prefix);

				$var = $field->isRepeated() ? "item" : "this.{$fname}";

				if(isEnum($field)) {
					$str1 = "WriteUtils.{$writer}(stream, (int){$var});";
				}
				else{
					$str1 = "WriteUtils.{$writer}(stream, {$var});";
				}
			%}
			{%if ($field->isRequired()):%}
			if (this.has{%$ufname%} {%if(isset($default)):%}|| this.{%$fname%}=={%$default%}{%endif;%}) {
                WriteUtils.writeTag(stream, WireType.{%$wireType%}, {%$number%});
                {%$str1%} 
			} else {
                throw new ProtobufException("Required field {%$fname%} not set");
            }
			{%elseif ($field->isRepeated()):%}
			if (this.has{%$ufname%}) {
				foreach (var item in this.{%$fname%})
                {
                    WriteUtils.writeTag(stream, WireType.{%$wireType%}, {%$number%});
                    {%$str1%} 
                }
			}
			{%else:%}
            {%if(isset($default)):%}
            if (this.has{%$ufname%} && this.{%$fname%} != {%$default%}) {
                WriteUtils.writeTag(stream, WireType.{%$wireType%}, {%$number%});
                {%$str1%} 
            }
            {%else:%}
            if (this.has{%$ufname%}) {
                WriteUtils.writeTag(stream, WireType.{%$wireType%}, {%$number%});
                {%$str1%} 
            }
            {%endif;%}
			{%endif;%}

			{%endforeach;%}
		}

		{%
	    	foreach ($enums as $enum) 
	    	{
	    		insertNestedEnum($enum);
	    	}
	   
	    	foreach ($nestedMessages as $msg) 
	    	{
	    		insertNestedClass($msg);
	    	}
	    %}
    
    }

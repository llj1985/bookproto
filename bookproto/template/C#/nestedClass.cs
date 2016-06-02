	public partial class {%echo $prefix.$name%} : NetMessage{%if($parentClass)echo ',',$parentClass;%} 
    {
		{%
			foreach ($fields as $field):
			$fname = $field->getName();
			$ufname = Ucfirst($fname);
			$number = $field->getNumber();
			$type = GetFieldType($field, $prefix);
            $default = $field->getDefault();
		%}
		private {%$type%} _{%$fname%}{%echo Isset($default)?'='.$default:''%};
		public  {%$type%} {%$fname%} 
		{
			get{return _{%$fname%};}
			set{_{%$fname%} = value;_has{%$ufname%}=true;}
		}

        private bool _has{%$ufname%} = false;
        {%if($field->isRepeated()):%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%} && _{%$fname%}!=null && _{%$fname%}.Count>0;}}
        {%elseif (IsMessage($field)):%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%} && _{%$fname%}!=null;}}
        {%else:%}
        public  bool has{%$ufname%} {get{return _has{%$ufname%};}}
        {%endif;%}
        {%if($field->isRepeated() || IsMessage($field)):%}
        public  void clear{%$ufname%}() {{%$fname%}=null; _has{%$ufname%}=false;}
        {%else:%}
		public  void clear{%$ufname%}() {{%echo Isset($default)?$fname.'='.$default.'; ':''%}_has{%$ufname%}=false;}
        {%endif;%}

		{%endforeach;%}

		public override void ParseFrom(ByteArray stream)
		{
			base.ParseFrom(stream);
			
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

            while (stream.bytesAvailable > 0)
            {
                var tag = ReadUtils.ReadTag(stream);
                switch (tag.number)
                {
                	case 0:
                        throw new ProtobufException("Invalid field id: 0, wrong stream");
					{%
						foreach ($fields as $field):
						$tag = $field->getNumber();
						$fname = $field->getName();
						$reader = GetFieldReader($field);
						$typeName = GetFieldType($field, $prefix);
						$typeName2 = GetFieldNotListType($field, $prefix);

						if (IsMessage($field)) {
                            $str1 = "ReadUtils.{$reader}(stream, new {$typeName2}()) as {$typeName2}";
						} else if(IsEnum($field)){
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
                        ReadUtils.Skip(stream, tag.wireType);
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

		public override void WriteTo(ByteArray stream)
		{
			base.WriteTo(stream);

            {%
				foreach ($fields as $field):
				$number = $field->getNumber();
				$fname = $field->getName();
				$ufname = Ucfirst($fname);
				$default = $field->getDefault();
				$writer = GetFieldWriter($field);
                $wireType = GetWireType($field);
				$typeName = GetFieldType($field, $prefix);
				$typeName2 = GetFieldNotListType($field, $prefix);

				$var = $field->isRepeated() ? "item" : "this.{$fname}";

				if(IsEnum($field)) {
					$str1 = "WriteUtils.{$writer}(stream, (int){$var});";
				}
				else{
					$str1 = "WriteUtils.{$writer}(stream, {$var});";
				}
			%}
			{%if ($field->isRequired()):%}
			if (this.has{%$ufname%} {%if(Isset($default)):%}|| this.{%$fname%}=={%$default%}{%endif;%}) {
                WriteUtils.WriteTag(stream, WireType.{%$wireType%}, {%$number%});
                {%$str1%} 
			} else {
                throw new ProtobufException("Required field {%$fname%} not set");
            }
			{%elseif ($field->isRepeated()):%}
			if (this.has{%$ufname%}) {
				foreach (var item in this.{%$fname%})
                {
                    WriteUtils.WriteTag(stream, WireType.{%$wireType%}, {%$number%});
                    {%$str1%} 
                }
			}
			{%else:%}
            {%if(Isset($default)):%}
            if (this.has{%$ufname%} && this.{%$fname%} != {%$default%}) {
                WriteUtils.WriteTag(stream, WireType.{%$wireType%}, {%$number%});
                {%$str1%} 
            }
            {%else:%}
            if (this.has{%$ufname%}) {
                WriteUtils.WriteTag(stream, WireType.{%$wireType%}, {%$number%});
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

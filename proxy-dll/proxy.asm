.code

extern OrignalDWriteCreateFactory:qword

DWriteCreateFactory proc EXPORT
  jmp OrignalDWriteCreateFactory
DWriteCreateFactory endp

end

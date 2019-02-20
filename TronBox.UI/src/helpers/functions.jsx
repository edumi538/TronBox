class Functions {

    static putRegistryMask(registry) {
        if (registry) {
            if (registry.length === 14) {
                return registry.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2}).*/, '$1.$2.$3/$4-$5');
            } else if (registry.length === 12) {
                return registry.replace(/^(\d{11})(\d{1}).*/, '$1-$2');
            } else {
                return registry.replace(/^(\d{3})(\d{3})(\d{3})(\d{2}).*/, '$1.$2.$3-$4');
            }
        }
    }

    static findObjectByKey(array, key, value) {
        for (var i = 0; i < array.length; i++) {
            if (array[i][key] === value) {
                return array[i];
            }
        }
        return null
    }

    static removeAcento(text)
    {
        text = text.toLowerCase();
        text = text.replace(new RegExp('[ÁÀÂÃ]','gi'), 'a')
        text = text.replace(new RegExp('[ÉÈÊ]','gi'), 'e')
        text = text.replace(new RegExp('[ÍÌÎ]','gi'), 'i')
        text = text.replace(new RegExp('[ÓÒÔÕ]','gi'), 'o')
        text = text.replace(new RegExp('[ÚÙÛ]','gi'), 'u')
        text = text.replace(new RegExp('[Ç]','gi'), 'c')
        return text
    }

    static validaCnpj(cnpj) {

           cnpj = cnpj.replace(/[^\d]+/g,'')

           if (cnpj == '') return false

           if (cnpj.length != 14)
               return false

           // Elimina CNPJs invalidos conhecidos
           if (cnpj == "00000000000000" ||
               cnpj == "11111111111111" ||
               cnpj == "22222222222222" ||
               cnpj == "33333333333333" ||
               cnpj == "44444444444444" ||
               cnpj == "55555555555555" ||
               cnpj == "66666666666666" ||
               cnpj == "77777777777777" ||
               cnpj == "88888888888888" ||
               cnpj == "99999999999999")
               return false

           // Valida DVs
           let tamanho = cnpj.length - 2
           let numeros = cnpj.substring(0,tamanho)
           let digitos = cnpj.substring(tamanho)
           let soma = 0
           let pos = tamanho - 7
           for (let i = tamanho; i >= 1; i--) {
             soma += numeros.charAt(tamanho - i) * pos--
             if (pos < 2)
                   pos = 9
           }
           resultado = soma % 11 < 2 ? 0 : 11 - soma % 11
           if (resultado != digitos.charAt(0))
               return false

           tamanho = tamanho + 1;
           numeros = cnpj.substring(0,tamanho)
           soma = 0
           pos = tamanho - 7
           for (let i = tamanho; i >= 1; i--) {
             soma += numeros.charAt(tamanho - i) * pos--
             if (pos < 2)
                   pos = 9
           }
           let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11
           if (resultado != digitos.charAt(1))
                 return false

           return true

       }
}

export default Functions

window.onload = function() {
    // 1. MAPEAMENTO INTELIGENTE (Busca os IDs de ambas as telas)
    const el = {
        btn: document.getElementById("btnCadastrar"),
        // Tenta achar o ID da tela de paciente ou da tela de terapeuta
        inputCPF: document.getElementById("cpf") || document.getElementById("cpfCadastro"),
        checkLGPD: document.getElementById("checkLGPD"),
        nome: document.getElementById("nome") || document.getElementById("nomeCadastro"),
        email: document.getElementById("email") || document.getElementById("emailCadastro"),
        senha: document.getElementById("senha") || document.getElementById("senhaCadastro"),
        especialidade: document.getElementById("especialidade"), // Apenas na tela de médico
        URL_API: "http://localhost:8000/register"
    };

    // 2. MÁSCARA DE CPF FUNCIONAL (Impede quebra de layout)
    if (el.inputCPF) {
        el.inputCPF.addEventListener("input", function(e) {
            let v = e.target.value.replace(/\D/g, ""); // Remove letras
            
            if (v.length > 11) v = v.substring(0, 11); // Trava em 11 números

            // Aplica os pontos e traço dinamicamente
            v = v.replace(/(\d{3})(\d)/, "$1.$2");
            v = v.replace(/(\d{3})(\d)/, "$1.$2");
            v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
            
            e.target.value = v;
        });
    }

    // 3. ATIVAÇÃO DO BOTÃO (LGPD)
    if (el.checkLGPD && el.btn) {
        el.checkLGPD.addEventListener("change", function() {
            el.btn.disabled = !this.checked;
            el.btn.style.opacity = this.checked ? "1" : "0.6";
            el.btn.style.cursor = this.checked ? "pointer" : "not-allowed";
        });
    }

    // 4. ENVIO DOS DADOS
    if (el.btn) {
        el.btn.onclick = async function(e) {
            e.preventDefault();

            const dados = {
                nome: el.nome?.value.trim(),
                cpf: el.inputCPF.value.replace(/\D/g, ""), // Envia apenas números
                email: el.email?.value.trim(),
                senha: el.senha?.value,
                especialidade: el.especialidade ? el.especialidade.value : null
            };

            // Validação de segurança
            if (!dados.nome || dados.cpf.length < 11 || !dados.email.includes("@") || dados.senha.length < 6) {
                alert("⚠️ Verifique os campos:\n- CPF completo\n- Email válido\n- Senha mínima de 6 caracteres");
                return;
            }

            try {
                const response = await fetch(el.URL_API, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(dados)
                });

                if (response.ok) {
                    alert("✅ Cadastro realizado com sucesso!");
                    window.location.href = "./login.html";
                } else {
                    const error = await response.json();
                    alert(error.detail || "Erro ao realizar cadastro.");
                }
            } catch (err) {
                console.error("Erro de conexão:", err);
                alert("❌ Erro ao conectar com o servidor.");
            }
        };
    }
};

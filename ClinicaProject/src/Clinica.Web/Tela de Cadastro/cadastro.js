window.onload = function() {
    const el = {
        btnCadastrar: document.getElementById("btnCadastrar"),
        inputNome: document.getElementById("nomeCadastro"),
        inputCPF: document.getElementById("cpf"),
        inputEmail: document.getElementById("emailCadastro"),
        inputSenha: document.getElementById("senhaCadastro"),
        checkLGPD: document.getElementById("checkLGPD"), // Novo elemento
        URL_API: "http://localhost:8000/register"
    };

    // --- 1. MÁSCARA DE CPF (Lógica Funcional) ---
    const aplicarMascaraCPF = (valor) => {
        return valor
            .replace(/\D/g, "") // Remove tudo que não é número
            .replace(/(\d{3})(\d)/, "$1.$2") // Coloca ponto após o 3º dígito
            .replace(/(\d{3})(\d)/, "$1.$2") // Coloca ponto após o 6º dígito
            .replace(/(\d{3})(\d{1,2})$/, "$1-$2") // Coloca traço após o 9º dígito
            .substring(0, 14); // Limita o tamanho total
    };

    if (el.inputCPF) {
        el.inputCPF.addEventListener("input", (e) => {
            e.target.value = aplicarMascaraCPF(e.target.value);
            el.inputCPF.classList.remove("input-erro");
        });
    }

    // --- 2. LÓGICA LGPD (Habilitar Botão) ---
    if (el.checkLGPD && el.btnCadastrar) {
        el.checkLGPD.addEventListener("change", () => {
            el.btnCadastrar.disabled = !el.checkLGPD.checked;
        });
    }

    // --- 3. ENVIO DO FORMULÁRIO ---
    const enviarCadastro = async () => {
        const dados = {
            nome: el.inputNome.value.trim(),
            cpf: el.inputCPF.value.replace(/\D/g, ""), // Limpa para a API
            email: el.inputEmail.value.trim(),
            senha: el.inputSenha.value.trim()
        };

        // Validações
        if (!dados.nome || dados.cpf.length !== 11 || !dados.email.includes("@") || dados.senha.length < 6) {
            alert("⚠️ Preencha todos os campos corretamente!");
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
                const erro = await response.json();
                alert(erro.detail || "Erro ao cadastrar.");
            }
        } catch (err) {
            console.error("Erro:", err);
            alert("Erro ao conectar com o servidor.");
        }
    };

    if (el.btnCadastrar) {
        el.btnCadastrar.onclick = (e) => {
            e.preventDefault();
            enviarCadastro();
        };
    }
};
function selecionarPerfil(tipo) {
    // Adiciona uma pequena animação de clique antes de redirecionar
    const card = tipo === 'paciente' ? 
        document.getElementById('cardPaciente') : 
        document.getElementById('cardProfissional');

    card.style.transform = "scale(0.95)";

    setTimeout(() => {
        if (tipo === 'paciente') {
            // Redireciona para a tela de cadastro de paciente
            window.location.href = "cadastro-paciente.html";
        } else {
            // Redireciona para a tela de cadastro de profissional
            window.location.href = "cadastro-profissional.html";
        }
    }, 150);
}

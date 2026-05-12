window.onload = function() {
    const el = {
        btnCadastrar: document.getElementById("btnCadastrar"),
        inputNome: document.getElementById("nomeCadastro"),
        inputCPF: document.getElementById("cpf"),
        inputEmail: document.getElementById("emailCadastro"),
        inputSenha: document.getElementById("senhaCadastro"),
        checkLGPD: document.getElementById("checkLGPD"), // Novo elemento
        URL_API: "http://localhost:8000/register",
        dialog: document.getElementById("modalSucessoCadastro"),
        btnRedirecionar: document.getElementById("btnRedirecionar")
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
            function validarCPF(cpf) {
            cpf = cpf.replace(/[^\d]+/g, ''); // Remove pontos e traços
            if (cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) return false; // Verifica tamanho e repetidos
            
            // Lógica matemática de validação
            let soma = 0;
            let resto;
            for (let i = 1; i <= 9; i++) soma = soma + parseInt(cpf.substring(i - 1, i)) * (11 - i);
            resto = (soma * 10) % 11;
            if ((resto === 10) || (resto === 11)) resto = 0;
            if (resto !== parseInt(cpf.substring(9, 10))) return false;
            
            soma = 0;
            for (let i = 1; i <= 10; i++) soma = soma + parseInt(cpf.substring(i - 1, i)) * (12 - i);
            resto = (soma * 10) % 11;
            if ((resto === 10) || (resto === 11)) resto = 0;
            if (resto !== parseInt(cpf.substring(10, 11))) return false;
            
            return true;
        }

        function checkForm() {
            let cpf = document.getElementById('cpf').value;
            if (!validarCPF(cpf)) {
                alert('CPF Inválido!');
                return false;
            }
            alert('CPF Válido!');
            return true;
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
                el.dialog.showModal();
            } else {
                const erro = await response.json();
                alert(erro.detail || "Erro ao cadastrar.");
                el.dialog.showModal();
            }
        } catch (err) {
            console.error("Erro:", err);
            alert("Erro ao conectar com o servidor.");
            el.dialog.showModal();
        }
    };
    ///4. EVENTOS///

    if (el.btnCadastrar) {
        el.btnCadastrar.onclick = (e) => {
            e.preventDefault();
            enviarCadastro();
        };

    }
        if  (el.btnRedirecionar) {
            el.btnRedirecionar.addEventListener("click", () =>{
                el.dialog.close();
                window.location.href = ("../tela de login/login.html");
        });
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

window.onload = function() {
    const btnAbrir = document.getElementById("btnAbrir");
    const btnFechar = document.getElementById("btnFechar");
    const modal = document.getElementById("modalAgenda");
    const modalSucesso = document.getElementById("modalSucesso");
    const btnConfirmar = document.getElementById("confirmarAgendamento");
    
    // Selects
    const selectPrincipal = document.getElementById('selectTerapeutaPrincipal');
    const selectTerapeutaModal = document.getElementById('selectTerapeuta');
    
    const inputData = document.getElementById('dataAgendamento');
    const containerHorarios = document.getElementById('containerHorarios');
    const textoResumo = document.getElementById('textoResumo');
    const dadosConfirmados = document.getElementById("dadosConfirmados");
    
    const toast = document.getElementById("toastErro");
    const progressBar = toast ? toast.querySelector('.toast-progress') : null;

    let agendamentoFinal = { terapeuta: '', data: '', hora: '' };
    let toastTimeout;

    // 2. ABRIR E FECHAR MODAL (Com sincronização)
    if (btnAbrir) {
        btnAbrir.onclick = () => {
            // Sincroniza o valor do select de fora para dentro do modal
            if (selectPrincipal && selectTerapeutaModal) {
                selectTerapeutaModal.value = selectPrincipal.value;
                // Dispara a busca de horários caso já tenha data preenchida
                buscarHorarios(selectTerapeutaModal.value, inputData.value);
            }
            modal.showModal();
        };
    }
    
    if (btnFechar) btnFechar.onclick = () => modal.close();

    // 3. BUSCAR HORÁRIOS
    async function buscarHorarios(terapeuta, data) {
        if (!terapeuta || !data) return;
        containerHorarios.innerHTML = "<p>Carregando horários...</p>";
        try {
            const respostaFake = ["08:00", "09:30", "11:00", "14:00", "15:30", "17:00"];
            containerHorarios.innerHTML = "";
            
            respostaFake.forEach(horario => {
                const btn = document.createElement("button");
                btn.type = "button";
                btn.className = "btn-hora";
                btn.innerText = horario;
                
                btn.onclick = function() {
                    document.querySelectorAll('.btn-hora').forEach(b => b.classList.remove('selecionado'));
                    this.classList.add('selecionado');
                    
                    agendamentoFinal.hora = horario;
                    agendamentoFinal.terapeuta = selectTerapeutaModal.options[selectTerapeutaModal.selectedIndex].text;
                    agendamentoFinal.data = inputData.value.split('-').reverse().join('/');
                    
                    textoResumo.innerText = `Selecionado: ${horario}`;
                };
                containerHorarios.appendChild(btn);
            });
        } catch (error) {
            containerHorarios.innerHTML = "<p>Erro ao carregar horários.</p>";
        }
    }

    if (selectTerapeutaModal && inputData) {
        selectTerapeutaModal.onchange = () => buscarHorarios(selectTerapeutaModal.value, inputData.value);
        inputData.onchange = () => buscarHorarios(selectTerapeutaModal.value, inputData.value);
    }

    // 4. LÓGICA DO TOAST/ERRO USUARIO AO CONFIRMAR SEM PREENCHER DADOS
    window.mostrarToast = function() {
        if (!toast) return;
        toast.classList.add("show");
        iniciarTimerToast();
    }

    function iniciarTimerToast() {
        clearTimeout(toastTimeout);
        toastTimeout = setTimeout(() => fecharToast(), 3000);
        if (progressBar) progressBar.classList.remove('pausar-animacao');
    }

    window.fecharToast = function() {
        if (toast) {
            toast.classList.remove("show");
            clearTimeout(toastTimeout);
        }
    }

    // 5. BOTÃO CONFIRMAR
    if (btnConfirmar) {
        btnConfirmar.onclick = function() {
            const botaoHoraAtivo = document.querySelector('.btn-hora.selecionado');

            if (selectTerapeutaModal.value === "" || inputData.value === "" || !botaoHoraAtivo) {
                mostrarToast(); 
                return;
            }

            dadosConfirmados.innerHTML = `
                <p><strong>Terapeuta/Médico:</strong> ${agendamentoFinal.terapeuta}</p>
                <p><strong>Data:</strong> ${agendamentoFinal.data}</p>
                <p><strong>Horário:</strong> ${agendamentoFinal.hora}</p>
            `;

            modal.close();
            modalSucesso.showModal();
        };
    }
};

# BiPi Entregas 🛵📦

> **BIPOU, CHEGOUUU!** — Sistema de gestão de entregas intermunicipais sob demanda.

O **BiPi Entregas** é um aplicativo móvel de logística sob demanda que conecta pessoas e lojistas que precisam **enviar ou receber encomendas** a **entregadores cadastrados e auditados**, com cálculo automático de frete, pagamento via Pix ou Cartão de Crédito e acompanhamento da entrega em tempo real.

---

## 📌 Contexto e problema

Segundo dados da ANTT, Sergipe registrou cerca de **89.900 operações de transporte de carga intermunicipais** somente em março de 2026. Apesar do volume, pequenas entregas entre cidades ainda dependem de soluções informais (contatos pessoais, topics, táxis, vans e cooperativas), sem padronização, rastreabilidade ou segurança jurídica. A informalidade no setor de fretes representa cerca de **43% do mercado nacional**, com perdas estimadas em **R$ 32 bilhões** em arrecadação tributária.

O BiPi Entregas propõe uma solução digital que estrutura essa intermediação, com mais **previsibilidade, rastreabilidade e confiabilidade**.

---

## 👥 Perfis de usuário

| Perfil | O que faz |
|---|---|
| **Solicitante** | Lojista ou pessoa física que solicita o envio (*Enviar*) ou o recebimento (*Receber*) de uma encomenda, paga e acompanha a entrega. |
| **Entregador** | Motorista parceiro auditado que aceita pedidos, coleta, transporta e finaliza entregas, recebendo o valor líquido em sua carteira. |
| **Administrador** | Colaborador da plataforma que audita documentos e aprova ou rejeita cadastros. |

> Um mesmo usuário pode acumular os perfis de Solicitante e Entregador e alternar entre eles sem novo login.

---

## ✨ Funcionalidades

### 🔐 Autenticação e cadastro
- Login com e-mail e senha ou **Google OAuth**
- Recuperação de senha
- Cadastro de Solicitante (dados pessoais, endereço e foto da fachada)
- Cadastro de Entregador (CNH, documento do veículo e dados bancários) com 

### 📦 Solicitante
- Solicitar entrega nas modalidades **Enviar** e **Receber**
- Informar endereços, foto, descrição, valor declarado, tamanho, peso e fragilidade
- Cálculo automático de distância, tempo estimado e frete
- Pagamento via **Cartão de Crédito** ou **Pix**, com valor retido em custódia até o aceite
- Gerenciamento de métodos de pagamento (adicionar, definir padrão e remover)
- Acompanhamento em tempo real com linha do tempo da entrega
- Contato direto com o entregador e **avaliação de 1 a 5 estrelas**
- Histórico de entregas e total gasto

### 🏍️ Entregador
- Modo **online** e radar de entregas disponíveis
- Aceite de **múltiplas entregas**, com bloqueio automático para os demais entregadores
- **Rota de coleta otimizada** entre os pontos aceitos
- Cancelamento gratuito dentro da janela de tolerância (bloqueado após iniciar a rota)
- Atualização das etapas: coleta concluída → transporte → entrega finalizada
- Painel de **ganhos** com saldo, gráfico semanal e saque para conta bancária

### 🛡️ Administrador
- Dashboard com indicadores de cadastros pendentes, aprovados, rejeitados e aguardando documentos
- Fila de cadastros pendentes com busca e filtros
- Aprovação ou rejeição com registro do motivo e notificação ao usuário

---

## 🔄 Ciclo de vida da solicitação

```
AguardandoPagamento → BuscandoEntregador → AguardandoColeta → Coletando → Transportando → Entregue
                              ↑                    │
                              └── cancelamento do entregador (dentro da janela gratuita)

Cancelada ← cancelamento do Solicitante (antes do aceite) ou do Entregador (fora da janela)
```

Regras importantes:
- Apenas **um entregador** pode aceitar cada solicitação.
- O entregador pode cancelar enquanto o pedido estiver em **"aceitos"**; após **iniciar a rota de coleta**, o cancelamento é bloqueado.
- Ao concluir a entrega, o solicitante avalia o serviço e o valor líquido é creditado ao entregador.

---

## 🧩 Casos de uso

| Código | Caso de uso | Ator principal |
|---|---|---|
| CSU01 | Manter Solicitação de Entrega | Solicitante |
| CSU02 | Autenticar Usuário | Todos |
| CSU03 | Manter Solicitante | Solicitante |
| CSU04 | Gerenciar Métodos de Pagamento | Solicitante |
| CSU05 | Acompanhar Entrega | Solicitante |
| CSU06 | Consultar Histórico | Solicitante / Entregador |
| CSU07 | Alternar Perfil | Solicitante / Entregador |
| CSU08 | Manter Entregador | Entregador |
| CSU09 | Atender Solicitação de Entrega | Entregador |
| CSU10 | Validar Cadastros Pendentes | Administrador |

**Atores secundários:** Gateway de Pagamento, Serviço GPS/Mapas e Google OAuth.

---

## 🗄️ Modelo de dados

Principais entidades do DER:

- **Usuario** (generalização) → **Solicitante**, **Entregador** e **Administrador**
- **MetodoPagamento** (generalização) → **CartaoCredito** e **Pix**
- **SolicitacaoEntrega**
- **Pagamento**
- **DadosBancarios**

---

## 📚 Documentação do projeto

| Artefato | Descrição |
|---|---|
| Documento de Visão (v1.5) | Posicionamento, stakeholders, requisitos funcionais e não funcionais, diagrama de casos de uso e máquina de estados |
| Especificação de Casos de Uso | CSU01 a CSU10 com fluxos principais, alternativos e de exceção |
| BPMN | Fluxos do solicitante, aceitação pelo entregador e execução da entrega |
| DER | Modelagem de dados (StarUML) |

> 💡 Sugestão: adicione os arquivos em uma pasta `docs/` e atualize os links desta tabela.

---

## 🛠️ Tecnologias

> _Preencha de acordo com a stack adotada pela equipe (aplicativo móvel, back-end, banco de dados, integrações)._

**Integrações previstas:** Google OAuth, serviço de mapas/GPS e gateway de pagamento (cartão e Pix).

---

## 🚀 Como executar

```bash
# Clone o repositório
git clone https://github.com/<usuario>/BiPiEntregas.git
cd BiPiEntregas

# Instruções de instalação e execução conforme a stack do projeto
```

---

## 👨‍💻 Equipe

- Igor Augusto
- Anderson Andrade
- Clara Prado
- Victor Pereira
- Caio Fidelis

---

## 📄 Licença

Este projeto está licenciado sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.

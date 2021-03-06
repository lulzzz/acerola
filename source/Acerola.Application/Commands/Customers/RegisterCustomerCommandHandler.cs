﻿namespace Acerola.Application.CommandHandlers.Customers
{
    using Acerola.Application.Commands.Customers;
    using MediatR;
    using System;
    using System.Threading.Tasks;
    using Acerola.Domain.Customers;
    using Acerola.Domain.ValueObjects;
    using Acerola.Domain.Accounts;

    public class RegisterCustomerCommandHandler : IAsyncRequestHandler<RegisterCustomerCommand, Customer>
    {
        private readonly ICustomerWriteOnlyRepository customerWriteOnlyRepository;
        private readonly IAccountWriteOnlyRepository accountWriteOnlyRepository;

        public RegisterCustomerCommandHandler(
            ICustomerWriteOnlyRepository customerWriteOnlyRepository,
            IAccountWriteOnlyRepository accountWriteOnlyRepository)
        {
            if (customerWriteOnlyRepository == null)
                throw new ArgumentNullException(nameof(customerWriteOnlyRepository));

            if (accountWriteOnlyRepository == null)
                throw new ArgumentNullException(nameof(accountWriteOnlyRepository));

            this.customerWriteOnlyRepository = customerWriteOnlyRepository;
            this.accountWriteOnlyRepository = accountWriteOnlyRepository;
        }

        public async Task<Customer> Handle(RegisterCustomerCommand command)
        {
            Customer customer = Customer.Create(
                PIN.Create(command.PIN),
                Name.Create(command.Name));

            Amount amount = Amount.Create(command.InitialAmount);

            Account account = Account.Create(
                customer,
                amount);

            customer.Register(account);

            await customerWriteOnlyRepository.Add(customer);
            await accountWriteOnlyRepository.Add(account);

            return customer;
        }
    }
}

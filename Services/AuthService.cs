using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models.Entities;
using LibraryManagementSystem.Utilities;

namespace LibraryManagementSystem.Services;

public class AuthService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;



    private UserEntity? currentUser;

    public UserEntity? CurrentUser
    {
        get { return currentUser; }
        private set
        {
            if (value == null && currentUser == null)
                return;

            if (value == null || currentUser == null)
            {
                currentUser = value;
                OnUserChange?.Invoke(null);
                return;

            }

            if (value!.Id != currentUser.Id)
            {
                currentUser = value;
                OnUserChange?.Invoke(currentUser);
            }
        }
    }

    public Action<UserEntity?>? OnUserChange;


    public bool EmailIsUsed(string email)
    {
        return _userRepository.GetUser(email) != null;
    }
    public Result<UserEntity> LoginWithEmailAndPassword(string email, string password)
    {
        if (!Validators.ValidateEmail(email))
            return Result<UserEntity>.Error("Invalid email format");

        if (!Validators.ValidatePassword(password))
            return Result<UserEntity>.Error("Invalid password format");

        UserEntity? fetchedUser = _userRepository.GetUser(email);

        //When user email is not found (not used)
        if (fetchedUser == null)
            return Result<UserEntity>.Error("Invalid email or password");


        //Compare hashed passwords
        byte[] salt = Convert.FromBase64String(fetchedUser.Salt);
        string hashed_password = Hashing.HashPassword(password, salt);

        //When passwords don't match
        if (fetchedUser.HashedPassword != hashed_password)
            return Result<UserEntity>.Error("Invalid email or password");

        CurrentUser = fetchedUser;

        return Result<UserEntity>.Success(CurrentUser);

    }
    public Result<UserEntity> RegisterWithEmailAndPassword(string email, string password, string firstName, string lastName, string? phone)
    {
        if (!Validators.ValidateEmail(email))
            return Result<UserEntity>.Error("Invalid email format");

        if (!Validators.ValidatePassword(password))
            return Result<UserEntity>.Error("Invalid password format");

        if (!Validators.ValidateName(firstName) || !Validators.ValidateName(lastName))
            return Result<UserEntity>.Error("Invalid name format");

        if (phone != null && !Validators.ValidatePhone(phone))
            return Result<UserEntity>.Error("Invalid phone number format");

        if (EmailIsUsed(email))
            return Result<UserEntity>.Error("Email already in use");

        byte[] salt = Hashing.GenerateSalt();
        string hashed_password = Hashing.HashPassword(password, salt);

        CurrentUser = _userRepository.CreateNewUser(email, firstName, lastName, phone, hashed_password, Convert.ToBase64String(salt));
        return Result<UserEntity>.Success(CurrentUser);

    }

    //Refreshes the current user data without invoking the OnUserChange
    public void RefetchCurrentUser()
    {
        if (currentUser is not null)
            currentUser = _userRepository.GetUser(currentUser!.Email);
    }

    public void Logout()
    {
        CurrentUser = null;
    }

}